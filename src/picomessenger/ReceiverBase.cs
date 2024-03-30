using System;
using System.Threading.Tasks;

namespace picomessenger
{
    public abstract class ReceiverBase<T> : IWrappedReceiver
    {
        public Task ReceiveAsync(T message)
        {
            return this.SendMessageAsync(message);
        }


        protected abstract Task SendMessageAsync(T message);

        public abstract IReceiver WrappedObject { get; }

        public Type MessageType { get; } = typeof(T);
    }
}