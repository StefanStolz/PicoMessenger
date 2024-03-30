using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace picomessenger
{
    public class PicoMessenger : IMessenger
    {
        private ImmutableArray<ReceiverBase> receivers = ImmutableArray.Create<ReceiverBase>();

        public int NumberOfRegisteredReceivers => this.receivers.Length;


        public void Register<T>(IReceiver<T> receiver)
        {
            Receiver<T> receiveHandler = new Receiver<T>(receiver);
            this.receivers = this.receivers.Add(receiveHandler);
        }

        public void Register<T>(IAsyncReceiver<T> receiver)
        {
            AsyncReceiver<T> receiveHandler = new AsyncReceiver<T>(receiver);
            this.receivers = this.receivers.Add(receiveHandler);
        }


        public void Deregister<T>(IReceiver<T> receiver) =>
            this.receivers = this.receivers.RemoveAll(x => ReferenceEquals(x.Item, receiver) &&
                                                           x.GetType().GetGenericArguments()[0] == typeof(T));

        public void Deregister<T>(IAsyncReceiver<T> receiver) =>
            this.receivers = this.receivers.RemoveAll(x => ReferenceEquals(x.Item, receiver) &&
                                                           x.GetType().GetGenericArguments()[0] == typeof(T));

        public void RegisterAll(IReceiver receiver)
        {
            Type[] interfaces = receiver.GetType().GetInterfaces();

            foreach (Type ifc in interfaces)
            {
                if (ifc.IsGenericType)
                {
                    if (ifc.GetGenericTypeDefinition() == typeof(IAsyncReceiver<>))
                    {
                        Type[] genericArguments = ifc.GetGenericArguments();
                        Type t = typeof(AsyncReceiver<>).MakeGenericType(genericArguments);

                        var receiverInstance = (ReceiverBase) Activator.CreateInstance(t, receiver);

                        this.receivers = this.receivers.Add(receiverInstance);
                    }
                    else if (ifc.GetGenericTypeDefinition() == typeof(IReceiver<>))
                    {
                        Type t = typeof(Receiver<>).MakeGenericType(ifc.GetGenericArguments());

                        var receiverInstance = (ReceiverBase) Activator.CreateInstance(t, receiver);

                        this.receivers = this.receivers.Add(receiverInstance);
                    }
                }
            }
        }

        public void DeregisterAll(IReceiver receiver) =>
            this.receivers = this.receivers.RemoveAll(x => ReferenceEquals(x.Item, receiver));


        public async Task SendMessageAsync<T>(T message)
        {
            foreach (ReceiverBase<T> receiver in this.receivers.OfType<ReceiverBase<T>>())
            {
                await receiver.ReceiveAsync(message);
            }
        }

        private abstract class ReceiverBase
        {
            public abstract IReceiver Item { get; }
        }

        private abstract class ReceiverBase<T> : ReceiverBase
        {
            public Task ReceiveAsync(T message)
            {
                return this.SendMessageAsync(message);
            }


            protected abstract Task SendMessageAsync(T message);
        }

        private sealed class Receiver<T> : ReceiverBase<T>
        {
            private readonly IReceiver<T> receiver;

            public Receiver(IReceiver<T> receiver)
            {
                this.receiver = receiver;
            }

            public override IReceiver Item => this.receiver;

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

            public override IReceiver Item => this.receiver;

            protected override Task SendMessageAsync(T message) => this.receiver.ReceiveAsync(message);
        }
    }
}