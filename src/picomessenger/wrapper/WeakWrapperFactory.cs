using System;
using System.Threading.Tasks;

namespace picomessenger.wrapper
{
    public class WeakWrapperFactory : IReceiverWrapperFactory
    {
        public IWrappedReceiver CreateWrappedReceiver(object receiver, Type receiverInterfaceType)
        {
            if (receiverInterfaceType.IsGenericType)
            {
                if (receiverInterfaceType.GetGenericTypeDefinition() == typeof(IAsyncReceiver<>))
                {
                    Type[] genericArguments = receiverInterfaceType.GetGenericArguments();
                    Type t = typeof(AsyncWeakReceiver<>).MakeGenericType(genericArguments);

                    return (IWrappedReceiver) Activator.CreateInstance(t, receiver);
                }

                if (receiverInterfaceType.GetGenericTypeDefinition() == typeof(IReceiver<>))
                {
                    Type[] genericArguments = receiverInterfaceType.GetGenericArguments();
                    Type t = typeof(WeakReceiver<>).MakeGenericType(genericArguments);

                    return (IWrappedReceiver) Activator.CreateInstance(t, receiver);
                }
            }

            throw new ArgumentException("Could not wrap Receiver");
        }

        private sealed class WeakReceiver<T> : WrappedReceiverBase<T>
        {
            private readonly WeakReference<IReceiver<T>> receiver;

            public WeakReceiver(IReceiver<T> receiver)
            {
                this.receiver = new WeakReference<IReceiver<T>>(receiver);
            }

            public override IReceiver WrappedObject
            {
                get
                {
                    if (this.receiver.TryGetTarget(out var target))
                    {
                        return target;
                    }

                    return null;
                }
            }

            public override bool IsAlive => this.receiver.TryGetTarget(out _);

            protected override Task SendMessageAsync(T message)
            {
                if (this.receiver.TryGetTarget(out var target))
                {
                    target.Receive(message);
                }

                return Task.CompletedTask;
            }
        }

        private sealed class AsyncWeakReceiver<T> : WrappedReceiverBase<T>
        {
            private readonly WeakReference<IAsyncReceiver<T>> receiver;

            public AsyncWeakReceiver(IAsyncReceiver<T> receiver)
            {
                this.receiver = new WeakReference<IAsyncReceiver<T>>(receiver);
            }

            public override IReceiver WrappedObject
            {
                get
                {
                    if (this.receiver.TryGetTarget(out var target))
                    {
                        return target;
                    }

                    return null;
                }
            }

            protected override Task SendMessageAsync(T message)
            {
                if (this.receiver.TryGetTarget(out var target))
                {
                    return target.ReceiveAsync(message);
                }
                
                return Task.CompletedTask;
            }

            public override bool IsAlive => this.receiver.TryGetTarget(out _);
        }
    }
}