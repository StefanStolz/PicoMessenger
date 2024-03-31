using System;
using System.Threading.Tasks;

namespace picomessenger.wrapper
{
    public interface IWrappedReceiver
    {
        bool IsAlive { get; }
        
        IReceiver WrappedObject { get; }

        Type MessageType { get; }
    }

    public interface IWrappedReceiver<T> : IWrappedReceiver
    {
        Task ReceiveAsync(T message);
    }
        
}