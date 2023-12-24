using System.Threading.Tasks;

namespace tinymessenger
{
    public interface IMessenger
    {
        IMessengerRegistration Register<T>(IReceiver<T> receiver);
        IMessengerRegistration Register<T>(IAsyncReceiver<T> receiver);
        void Deregister<T>(IReceiver<T> receiver);
        void Deregister<T>(IAsyncReceiver<T> receiver);

        void RegisterAll(IReceiver receiver);
        void DeregisterAll(IReceiver receiver);

        void Send<T>(T message);
        Task SendAsync<T>(T message);
    }
}