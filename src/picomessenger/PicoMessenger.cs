using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace picomessenger
{
    public class PicoMessenger : IMessenger
    {
        private ImmutableArray<ReceiverBase> receivers = ImmutableArray.Create<ReceiverBase>();

        public int NumberOfRegisteredReceivers => this.receivers.Length;


        public IMessengerRegistration Register<T>(IReceiver<T> receiver)
        {
            Receiver<T> receiveHandler = new Receiver<T>(receiver);
            this.receivers = this.receivers.Add(receiveHandler);

            return receiveHandler;
        }

        public IMessengerRegistration Register<T>(IAsyncReceiver<T> receiver)
        {
            AsyncReceiver<T> receiveHandler = new AsyncReceiver<T>(receiver);
            this.receivers = this.receivers.Add(receiveHandler);

            return receiveHandler;
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

                        ReceiverBase recv = (ReceiverBase) Activator.CreateInstance(t, receiver);

                        this.receivers = this.receivers.Add(recv);
                    }
                    else if (ifc.GetGenericTypeDefinition() == typeof(IReceiver<>))
                    {
                        Type t = typeof(Receiver<>).MakeGenericType(ifc.GetGenericArguments());

                        ReceiverBase recv = (ReceiverBase) Activator.CreateInstance(t, receiver);

                        this.receivers = this.receivers.Add(recv);
                    }
                }
            }
        }

        public void DeregisterAll(IReceiver receiver) =>
            this.receivers = this.receivers.RemoveAll(x => ReferenceEquals(x.Item, receiver));


        public async void Send<T>(T message) => await this.SendAsync(message);

        public async Task SendAsync<T>(T message)
        {
            foreach (ReceiverBase<T> receiver in this.receivers.OfType<ReceiverBase<T>>())
            {
                try
                {
                    await receiver.ReceiveAsync(message);
                }
                catch (Exception exception)
                {
                    if (!await receiver.ExceptionOcurredAsync(exception))
                    {
                        throw;
                    }
                }
            }
        }

        private abstract class ReceiverBase
        {
            public abstract IReceiver Item { get; }
        }

        private abstract class ReceiverBase<T> : ReceiverBase, IMessengerRegistration
        {
            private bool disabled;
            private Func<Exception, Task<MessengerErrorPolicy>> errorHandler;

            public void SetErrorHandler(Func<Exception, Task<MessengerErrorPolicy>> handler) =>
                this.errorHandler = handler ?? throw new ArgumentNullException(nameof(handler));

            public MessengerErrorPolicy ErrorPolicy { get; set; }

            public Task ReceiveAsync(T message)
            {
                if (this.disabled)
                {
                    return Task.CompletedTask;
                }

                return this.SendMessageAsync(message);
            }


            protected abstract Task SendMessageAsync(T message);


            /// <summary>
            /// </summary>
            /// <param name="exception"></param>
            /// <returns><c>true</c> if handled; otherwise <c>false</c></returns>
            /// <exception cref="NotImplementedException"></exception>
            public async Task<bool> ExceptionOcurredAsync(Exception exception)
            {
                MessengerErrorPolicy policy = this.ErrorPolicy;

                if (this.errorHandler != null)
                {
                    policy = await this.errorHandler(exception);
                }

                switch (policy)
                {
                    case MessengerErrorPolicy.Throw:
                        return false;
                    case MessengerErrorPolicy.DisableReceiver:
                        this.disabled = true;
                        return true;
                    case MessengerErrorPolicy.Ignore:
                        return true;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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

    public interface IReceiver<T> : IReceiver
    {
        void Receive(T message);
    }
}