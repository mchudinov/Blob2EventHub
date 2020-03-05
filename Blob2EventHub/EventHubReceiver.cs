using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Logging;

namespace Blob2EventHub
{
    public class EventHubReceiver : IDataReceiver
    {
        private readonly EventHubClient _eventHubClient;
        private readonly ILogger<EventHubReceiver> _logger;

        public EventHubReceiver(Configuration config, ILoggerFactory loggerFactory=null)
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(config.EventHubConnectionString + $";EntityPath={config.EventHubName}");
            _eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            _logger = loggerFactory?.CreateLogger<EventHubReceiver>();
        }

        public Task ReceiveAsync(string message)
        {
            return _eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
        }

        public async Task ReceiveAsync(byte[] bytearray)
        {
            try
            {
                await _eventHubClient.SendAsync(new EventData(bytearray));
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex.Message);
            }
        }
    }
}
