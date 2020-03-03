using System.Threading.Tasks;

namespace Blob2EventHub
{
    public interface IDataReceiver
    {
        Task ReceiveAsync(string message);
        Task ReceiveAsync(byte[] bytearray);
    }
}
