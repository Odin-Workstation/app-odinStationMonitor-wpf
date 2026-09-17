using Jendamark.Messaging;
using Jendamark.Messaging.ZRE;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace app_odinStationMonitor_Messaging
{
    public class ZREMessagingStarter
    {
        private ZREProtocolController _protocolController;
        private IMessageValidator _validator = new MessageValidator();

        public ZREMessenger Messenger { get; private set; }

        public ZREMessagingStarter(ILogger logger, string defaultName, string netInterface = "*", ushort broadcastPort = 5670, int broadcastInterval = 1, HashSet<string> domains = default, int messageIdPrefix = 0)
        {
            _protocolController = new ZREProtocolController(defaultName, netInterface, broadcastPort, broadcastInterval, domains, logger);
            Messenger = new ZREMessenger(_validator, _protocolController, logger, messageIdPrefix);
        }

        public void Start()
        {
            _protocolController.ServiceTaskRunning += ProtocolController_ServiceTaskRunning;

            _protocolController.StartController();

        }

        private void ProtocolController_ServiceTaskRunning(object sender, EventArgs e)
        {
            _protocolController.StartBroadcasting();
        }

        public void Stop()
        {
            _protocolController.StopBroacasting();
            _protocolController.Dispose();
        }
    }
}
