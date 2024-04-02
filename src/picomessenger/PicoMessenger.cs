using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using picomessenger.wrapper;

namespace picomessenger
{
    public class PicoMessenger : IMessenger
    {
        private ImmutableArray<IWrappedReceiver> receivers = ImmutableArray.Create<IWrappedReceiver>();

        private readonly IReceiverWrapperFactory defaultWrapperFactory;

        public PicoMessenger(IReceiverWrapperFactory receiverDefaultWrapperFactory)
        {
            this.defaultWrapperFactory = receiverDefaultWrapperFactory ??
                                         throw new ArgumentNullException(nameof(receiverDefaultWrapperFactory));
        }

        public PicoMessenger()
            : this(new SimpleReceiverWrapperFactory())
        { }

        public int NumberOfRegisteredReceivers => this.receivers.Length;

        /// <inheritdoc />
        public void Register<T>(IReceiver<T> receiver, IReceiverWrapperFactory wrapperFactory)
        {
            if (receiver == null)
                throw new ArgumentNullException(nameof(receiver));

            if (wrapperFactory == null)
                throw new ArgumentNullException(nameof(wrapperFactory));


            this.RegisterSingle(receiver, wrapperFactory);
        }

        /// <inheritdoc />
        public void Register<T>(IReceiver<T> receiver)
        {
            if (receiver == null)
                throw new ArgumentNullException(nameof(receiver));

            this.RegisterSingle(receiver, this.defaultWrapperFactory);
        }

        /// <inheritdoc />
        public void Register<T>(IAsyncReceiver<T> receiver, IReceiverWrapperFactory wrapperFactory)
        {
            if (receiver == null)
                throw new ArgumentNullException(nameof(receiver));

            if (wrapperFactory == null)
                throw new ArgumentNullException(nameof(wrapperFactory));

            this.RegisterSingle(receiver, wrapperFactory);
        }

        /// <inheritdoc />
        public void Register<T>(IAsyncReceiver<T> receiver)
        {
            if (receiver == null)
                throw new ArgumentNullException(nameof(receiver));

            this.RegisterSingle(receiver, this.defaultWrapperFactory);
        }

        /// <inheritdoc />
        public void Deregister<T>(IReceiver<T> receiver)
        {
            if (receiver == null)
                throw new ArgumentNullException(nameof(receiver));

            this.receivers = this.receivers.RemoveAll(x => ReferenceEquals(x.WrappedObject, receiver) &&
                                                           x.MessageType == typeof(T));
        }

        /// <inheritdoc />
        public void Deregister<T>(IAsyncReceiver<T> receiver)
        {
            if (receiver == null)
                throw new ArgumentNullException(nameof(receiver));

            this.receivers = this.receivers.RemoveAll(x => ReferenceEquals(x.WrappedObject, receiver) &&
                                                           x.MessageType == typeof(T));
        }

        private void RegisterSingle<T>(T receiver, IReceiverWrapperFactory wrapperFactory) where T : IReceiver
        {
            Type receiverInterfaceType = typeof(T);
            var wrappedReceiver = wrapperFactory.CreateWrappedReceiver(receiver, receiverInterfaceType);

            this.receivers = this.receivers.Add(wrappedReceiver);
        }

        /// <inheritdoc />
        public void RegisterAll(IReceiver receiver)
        {
            if (receiver == null)
                throw new ArgumentNullException(nameof(receiver));

            this.RegisterAll(receiver, this.defaultWrapperFactory);
        }

        /// <inheritdoc />
        public void RegisterAll(IReceiver receiver, IReceiverWrapperFactory wrapperFactory)
        {
            if (receiver == null)
                throw new ArgumentNullException(nameof(receiver));

            if (wrapperFactory == null)
                throw new ArgumentNullException(nameof(wrapperFactory));

            Type[] interfaces = receiver.GetType().GetInterfaces();

            foreach (Type ifc in interfaces)
            {
                if (ifc.IsGenericType && typeof(IReceiver).IsAssignableFrom(ifc))
                {
                    var wrappedReceiver = wrapperFactory.CreateWrappedReceiver(receiver, ifc);

                    this.receivers = this.receivers.Add(wrappedReceiver);
                }
            }
        }

        /// <inheritdoc />
        public void DeregisterAll(IReceiver receiver)
        {
            this.receivers = this.receivers.RemoveAll(x => ReferenceEquals(x.WrappedObject, receiver));
        }

        /// <inheritdoc />
        public async Task SendMessageAsync<T>(T message)
        {
            await Task.WhenAll(
                this.receivers.Where(r => r.IsAlive)
                    .OfType<IWrappedReceiver<T>>()
                    .Select(r => r.ReceiveAsync(message)));
        }
    }
}