using System;
using System.Threading.Tasks;

namespace picomessenger.wrapper
{
    public class SimpleReceiverWrapperFactory : IReceiverWrapperFactory
    {
        public IWrappedReceiver CreateWrappedReceiver(object receiver, Type receiverInterfaceType)
        {
            if (receiverInterfaceType.IsGenericType)
            {
                if (receiverInterfaceType.GetGenericTypeDefinition() == typeof(IAsyncReceiver<>))
                {
                    Type[] genericArguments = receiverInterfaceType.GetGenericArguments();
                    Type t = typeof(AsyncWrappedReceiver<>).MakeGenericType(genericArguments);

                    return (IWrappedReceiver) Activator.CreateInstance(t, receiver);
                }

                if (receiverInterfaceType.GetGenericTypeDefinition() == typeof(IReceiver<>))
                {
                    Type[] genericArguments = receiverInterfaceType.GetGenericArguments();
                    Type t = typeof(WrappedReceiver<>).MakeGenericType(genericArguments);

                    return (IWrappedReceiver) Activator.CreateInstance(t, receiver);
                }
            }

            throw new ArgumentException("Could not wrap Receiver");
        }

        private sealed class WrappedReceiver<T> : WrappedReceiverBase<T>
        {
            private readonly IReceiver<T> receiver;

            public WrappedReceiver(IReceiver<T> receiver)
            {
                this.receiver = receiver;
            }

            public override IReceiver WrappedObject => this.receiver;

            public override bool IsAlive { get; } = true;

            protected override Task SendMessageAsync(T message)
            {
                this.receiver.Receive(message);
                return Task.CompletedTask;
            }
        }

        private sealed class AsyncWrappedReceiver<T> : WrappedReceiverBase<T>
        {
            private readonly IAsyncReceiver<T> receiver;

            public AsyncWrappedReceiver(IAsyncReceiver<T> receiver)
            {
                this.receiver = receiver;
            }

            public override IReceiver WrappedObject => this.receiver;
            public override bool IsAlive { get; } = true;

            protected override Task SendMessageAsync(T message) => this.receiver.ReceiveAsync(message);
        }
    }
}