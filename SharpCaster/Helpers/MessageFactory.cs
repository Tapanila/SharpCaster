using System;

namespace SharpCaster.Helpers
{
    internal static class MessageFactory
    {

        public static class DialConstants
        {
            public const string DialMultiScreenUrn = "urn:dial-multiscreen-org:service:dial:1";
            public const string DialConnectionUrn = "urn:x-cast:com.google.cast.tp.connection";
            public const string DialHeartbeatUrn = "urn:x-cast:com.google.cast.tp.heartbeat";
            public const string DialReceiverUrn = "urn:x-cast:com.google.cast.receiver";
            public const string DialMediaUrn = "urn:x-cast:com.google.cast.media";
            public const string MagineUrn = "urn:x-cast:com.magine.cast.state";
        }

        public static CastMessage Generic()
        {
            return new CastMessage()
            {
                ProtocolVersion = 0,
                PayloadType = 0,
                PayloadBinary = null,
                DestinationId = "receiver-0",
                SourceId = "sender-0"
            };
        }

        public static CastMessage GenericWithID(string dest = "receiver-0")
        {
            return new CastMessage()
            {
                ProtocolVersion = 0,
                PayloadType = 0,
                PayloadBinary = null,
                DestinationId = dest,
                SourceId = uniqueSourceID
            };
        }

        private static string uniqueSourceID = "client-" + new Random((int)DateTime.Now.Ticks).Next() % 9999;

        public static CastMessage Close()
        {
            var msg = Generic();
            msg.Namespace = DialConstants.DialConnectionUrn;
            msg.PayloadUtf8 = new CloseRequest().ToJson();
            return msg;
        }
        public static CastMessage Connect()
        {
            var msg = Generic();
            msg.Namespace = DialConstants.DialConnectionUrn;
            msg.PayloadUtf8 = new ConnectRequest().ToJson();
            return msg;
        }

        public static CastMessage Connect(string destinationId)
        {
            var msg = GenericWithID(destinationId);
            msg.Namespace = DialConstants.DialConnectionUrn;
            msg.PayloadUtf8 = new ConnectRequest().ToJson();
            return msg;
        }
        public static CastMessage Ping()
        {
            var msg = Generic();
            msg.Namespace = DialConstants.DialHeartbeatUrn;
            msg.PayloadUtf8 = new PingRequest().ToJson();
            return msg;
        }
        public static CastMessage Pong()
        {
            var msg = Generic();
            msg.Namespace = DialConstants.DialHeartbeatUrn;
            msg.PayloadUtf8 = new PongRequest().ToJson();
            return msg;
        }
        public static CastMessage Status()
        {
            var msg = Generic();
            msg.Namespace = DialConstants.DialReceiverUrn;
            msg.PayloadUtf8 = new GetStatusRequest().ToJson();
            return msg;
        } 
        
        public static CastMessage Play(string destinationId)
        {
            var msg = GenericWithID(destinationId);
            msg.Namespace = DialConstants.DialMediaUrn;
            msg.PayloadUtf8 = new PlayRequest().ToJson();
            return msg;
        }
        public static CastMessage Pause(string destinationId)
        {
            var msg = GenericWithID(destinationId);
            msg.Namespace = DialConstants.DialMediaUrn;
            msg.PayloadUtf8 = new PauseRequest().ToJson();
            return msg;
        }
        public static CastMessage Launch(string appId)
        {
            var msg = Generic();
            msg.Namespace = DialConstants.DialReceiverUrn;
            msg.PayloadUtf8 = new LaunchRequest(appId).ToJson();
            return msg;
        }
        public static CastMessage Load(string destinationId, string payload)
        {
            var msg = GenericWithID(destinationId);
            msg.Namespace = DialConstants.DialMediaUrn;
            msg.PayloadUtf8 = payload;
            return msg;
        }

    }
}
