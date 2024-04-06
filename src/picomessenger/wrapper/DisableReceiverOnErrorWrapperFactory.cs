using System;
using System.Threading.Tasks;

namespace picomessenger.wrapper
{
    [Obsolete("Will be removed; use ConfigureableWrapperFactory")]
    public class DisableReceiverOnErrorWrapperFactory : IReceiverWrapperFactory
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

        private abstract class DisableableWrappedReceiverBase<T> : WrappedReceiverBase<T>
        {
            private bool disabled;

            protected sealed override async Task SendMessageAsync(T message)
            {
                if (!this.disabled)
                {
                    try
                    {
                        await this.SendMessageAsyncInternal(message);
                    }
                    catch (Exception)
                    {
                        this.disabled = true;
                    }
                }
            }

            protected abstract Task SendMessageAsyncInternal(T message);

            public override bool IsAlive { get; } = true;
        }

        private sealed class WrappedReceiver<T> : DisableableWrappedReceiverBase<T>
        {
            private readonly IReceiver<T> receiver;

            public WrappedReceiver(IReceiver<T> receiver)
            {
                this.receiver = receiver;
            }

            public override IReceiver WrappedObject => this.receiver;

            protected override Task SendMessageAsyncInternal(T message)
            {
                this.receiver.Receive(message);
                return Task.CompletedTask;
            }
        }

        private sealed class AsyncWrappedReceiver<T> : DisableableWrappedReceiverBase<T>
        {
            private readonly IAsyncReceiver<T> receiver;

            public AsyncWrappedReceiver(IAsyncReceiver<T> receiver)
            {
                this.receiver = receiver;
            }

            public override IReceiver WrappedObject => this.receiver;

            protected override Task SendMessageAsyncInternal(T message)
            {
                return this.receiver.ReceiveAsync(message);
            }
        }
    }
}