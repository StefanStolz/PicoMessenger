using System.Threading.Tasks;

namespace tinymessenger
{
    public interface IAsyncReceiver<T> : IReceiver
    {
        Task ReceiveAsync(T message);
    }
}