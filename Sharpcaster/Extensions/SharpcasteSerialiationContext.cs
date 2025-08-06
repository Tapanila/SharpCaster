using Sharpcaster.Interfaces;
using Sharpcaster.Messages;
using Sharpcaster.Messages.Connection;
using Sharpcaster.Messages.Heartbeat;
using Sharpcaster.Messages.Media;
using Sharpcaster.Messages.Multizone;
using Sharpcaster.Messages.Queue;
using Sharpcaster.Messages.Receiver;
using Sharpcaster.Messages.Spotify;
using Sharpcaster.Messages.Web;
using Sharpcaster.Models.Media;
using Sharpcaster.Models.ChromecastStatus;
using Sharpcaster.Models.Cast;
using System.Text.Json.Serialization;

namespace Sharpcaster.Extensions
{

    [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
    [JsonSerializable(typeof(ConnectMessage))]
    [JsonSerializable(typeof(PingMessage))]
    [JsonSerializable(typeof(PongMessage))]
    [JsonSerializable(typeof(IMessage))]
    [JsonSerializable(typeof(Message))]
    [JsonSerializable(typeof(MessageWithId))]
    [JsonSerializable(typeof(GetStatusMessage))]
    [JsonSerializable(typeof(ReceiverStatusMessage))]
    [JsonSerializable(typeof(LaunchMessage))]
    [JsonSerializable(typeof(LaunchStatusMessage))]
    [JsonSerializable(typeof(LaunchErrorMessage))]
    [JsonSerializable(typeof(PlayMessage))]
    [JsonSerializable(typeof(PauseMessage))]
    [JsonSerializable(typeof(SeekMessage))]
    [JsonSerializable(typeof(StopMediaMessage))]
    [JsonSerializable(typeof(StopMessage))]
    [JsonSerializable(typeof(MediaSessionMessage))]
    [JsonSerializable(typeof(MediaStatusMessage))]
    [JsonSerializable(typeof(MultizoneStatusMessage))]
    [JsonSerializable(typeof(CloseMessage))]
    [JsonSerializable(typeof(LoadMessage))]
    [JsonSerializable(typeof(QueueInsertMessage))]
    [JsonSerializable(typeof(QueueRemoveMessage))]
    [JsonSerializable(typeof(QueueReorderMessage))]
    [JsonSerializable(typeof(QueueUpdateMessage))]
    [JsonSerializable(typeof(QueueLoadMessage))]
    [JsonSerializable(typeof(QueueGetItemsMessage))]
    [JsonSerializable(typeof(QueueItemsMessage))]
    [JsonSerializable(typeof(QueueGetItemIdsMessage))]
    [JsonSerializable(typeof(QueueItemIdsMessage))]
    [JsonSerializable(typeof(AddUserResponseMessage))]
    [JsonSerializable(typeof(GetInfoResponseMessage))]
    [JsonSerializable(typeof(GetInfoMessage))]
    [JsonSerializable(typeof(AddUserMessage))]
    [JsonSerializable(typeof(DeviceUpdatedMessage))]
    [JsonSerializable(typeof(LoadFailedMessage))]
    [JsonSerializable(typeof(LoadCancelledMessage))]
    [JsonSerializable(typeof(InvalidRequestMessage))]
    [JsonSerializable(typeof(ErrorMessage))]
    [JsonSerializable(typeof(SetVolumeMessage))]
    [JsonSerializable(typeof(Track))]
    [JsonSerializable(typeof(TrackType))]
    [JsonSerializable(typeof(TextTrackType))]
    [JsonSerializable(typeof(TextTrackStyle))]
    [JsonSerializable(typeof(LiveSeekableRange))]
    [JsonSerializable(typeof(VideoInformation))]
    [JsonSerializable(typeof(BreakStatus))]
    [JsonSerializable(typeof(HdrType))]
    [JsonSerializable(typeof(IdleReason))]
    [JsonSerializable(typeof(MediaCommand))]
    [JsonSerializable(typeof(ResumeState))]
    [JsonSerializable(typeof(Break))]
    [JsonSerializable(typeof(BreakClip))]
    [JsonSerializable(typeof(VastAdsRequest))]
    [JsonSerializable(typeof(HlsSegmentFormat))]
    [JsonSerializable(typeof(UserAction))]
    [JsonSerializable(typeof(UserActionState))]
    [JsonSerializable(typeof(ContainerType))]
    [JsonSerializable(typeof(ContainerMetadata))]
    [JsonSerializable(typeof(QueueType))]
    [JsonSerializable(typeof(HlsVideoSegmentFormat))]
    [JsonSerializable(typeof(Capability))]
    [JsonSerializable(typeof(ReceiverType))]
    [JsonSerializable(typeof(SessionStatus))]
    [JsonSerializable(typeof(VolumeControlType))]
    [JsonSerializable(typeof(ErrorCode))]
    [JsonSerializable(typeof(ReceiverAvailability))]
    [JsonSerializable(typeof(AutoJoinPolicy))]
    [JsonSerializable(typeof(DefaultActionPolicy))]
    [JsonSerializable(typeof(SenderPlatform))]
    [JsonSerializable(typeof(ReceiverAction))]
    [JsonSerializable(typeof(SkipAdMessage))]
    [JsonSerializable(typeof(SetPlaybackRateMessage))]
    [JsonSerializable(typeof(UserActionMessage))]
    [JsonSerializable(typeof(EditTracksInfoMessage))]
    [JsonSerializable(typeof(WebMessage))]
    public partial class SharpcasteSerializationContext : JsonSerializerContext
    {
    }
}
