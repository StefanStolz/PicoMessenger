using System.Threading.Tasks;

namespace picomessenger
{
    public interface IAsyncReceiver<T> : IReceiver
    {
        Task ReceiveAsync(T message);
    }
}