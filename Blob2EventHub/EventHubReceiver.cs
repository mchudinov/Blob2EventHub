using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;

namespace Blob2EventHub
{
    public class EventHubReceiver : IDataReceiver
    {
        private readonly EventHubClient _eventHubClient;

        public EventHubReceiver(Configuration config)
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(config.EventHubConnectionString + $";EntityPath={config.EventHubName}");
            _eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
        }

        public Task ReceiveAsync(string message)
        {
            return _eventHubClient.SendAsync(new EventData(Encoding.UTF8.GetBytes(message)));
        }

        public Task ReceiveAsync(byte[] bytearray)
        {
            return _eventHubClient.SendAsync(new EventData(bytearray));
        }
    }
}
