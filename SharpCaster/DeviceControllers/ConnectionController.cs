using SharpCaster.Interfaces;
using SharpCaster.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpCaster.DeviceControllers
{
    public class ConnectionController : IConnectionController
    {
        private ChromeCastClient _chromecastClient { get; set; }
        private ChromecastChannel _connectionChannel { get; set; }

        public ConnectionController(ChromeCastClient chromecastClient)
        {
            _chromecastClient = chromecastClient;
            _connectionChannel = _chromecastClient.CreateChannel(MessageFactory.DialConstants.DialConnectionUrn);
        }

        public async Task OpenConnection()
        {
            await _connectionChannel.Write(MessageFactory.Connect());
        }

        public async Task<bool> ConnectToApplication(string applicationId)
        {
            var startedApplication = _chromecastClient.ChromecastStatus?.Applications?.FirstOrDefault(x => x.AppId == applicationId);
            if (startedApplication == null) return false;
            if (!string.IsNullOrWhiteSpace(_chromecastClient.CurrentApplicationSessionId)) return false;

            _chromecastClient.UpdateApplicationConnectionIds(startedApplication.SessionId,startedApplication.TransportId);

            await _chromecastClient.ChromecastSocketService.Write(MessageFactory.ConnectWithDestination(startedApplication.TransportId).ToProto());

            _chromecastClient.InvokeApplicationStarted(startedApplication);

            return true;
        }
    }
}
