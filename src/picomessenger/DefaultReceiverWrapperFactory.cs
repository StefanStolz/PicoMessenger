using System;
using System.Threading.Tasks;

namespace picomessenger
{
    public class DefaultReceiverWrapperFactory : IReceiverWrapperFactory
    {
        public IWrappedReceiver CreateWrappedReceiver(object receiver, Type receiverInterfaceType)
        {
            if (receiverInterfaceType.IsGenericType)
            {
                if (receiverInterfaceType.GetGenericTypeDefinition() == typeof(IAsyncReceiver<>))
                {
                    Type[] genericArguments = receiverInterfaceType.GetGenericArguments();
                    Type t = typeof(AsyncReceiver<>).MakeGenericType(genericArguments);

                    var wrappedReceiver = (IWrappedReceiver) Activator.CreateInstance(t, receiver);

                    return wrappedReceiver;
                }

                if (receiverInterfaceType.GetGenericTypeDefinition() == typeof(IReceiver<>))
                {
                    Type t = typeof(Receiver<>).MakeGenericType(receiverInterfaceType.GetGenericArguments());

                    var wrappedReceiver = (IWrappedReceiver) Activator.CreateInstance(t, receiver);

                    return wrappedReceiver;
                }
            }

            throw new ArgumentException("Could not wrap Receiver");

        }
        
        private sealed class Receiver<T> : ReceiverBase<T>
        {
            private readonly IReceiver<T> receiver;

            public Receiver(IReceiver<T> receiver)
            {
                this.receiver = receiver;
            }

            public override IReceiver WrappedObject => this.receiver;

            protected override Task SendMessageAsync(T message)
            {
                this.receiver.Receive(message);
                return Task.CompletedTask;
            }
        }

        private sealed class AsyncReceiver<T> : ReceiverBase<T>
        {
            private readonly IAsyncReceiver<T> receiver;

            public AsyncReceiver(IAsyncReceiver<T> receiver)
            {
                this.receiver = receiver;
            }

            public override IReceiver WrappedObject => this.receiver;

            protected override Task SendMessageAsync(T message) => this.receiver.ReceiveAsync(message);
        }
    }
}