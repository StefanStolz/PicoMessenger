using System;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace picomessenger
{
    public class PicoMessenger : IMessenger
    {
        private ImmutableArray<IWrappedReceiver> receivers = ImmutableArray.Create<IWrappedReceiver>();

        private IReceiverWrapperFactory wrapperFactory = new DefaultReceiverWrapperFactory();
        
        public int NumberOfRegisteredReceivers => this.receivers.Length;

        public void Register<T>(IReceiver<T> receiver)
        {
            this.RegisterSingle(receiver);
        }

        public void Register<T>(IAsyncReceiver<T> receiver)
        {
            this.RegisterSingle(receiver);
        }

        public void Deregister<T>(IReceiver<T> receiver)
        {
            this.receivers = this.receivers.RemoveAll(x => ReferenceEquals(x.WrappedObject, receiver) &&
                                                           x.MessageType == typeof(T));
        }

        public void Deregister<T>(IAsyncReceiver<T> receiver)
        {
            this.receivers = this.receivers.RemoveAll(x => ReferenceEquals(x.WrappedObject, receiver) &&
                                                           x.MessageType == typeof(T));
        }

        private void RegisterSingle<T>(T receiver) where T : IReceiver
        {
            Type receiverInterfaceType = typeof(T);
            var wrappedReceiver = this.wrapperFactory.CreateWrappedReceiver(receiver, receiverInterfaceType);

            this.receivers = this.receivers.Add(wrappedReceiver);
        }

        public void RegisterAll(IReceiver receiver)
        {
            Type[] interfaces = receiver.GetType().GetInterfaces();

            foreach (Type ifc in interfaces)
            {
                if (ifc.IsGenericType && typeof(IReceiver).IsAssignableFrom(ifc))
                {
                    var wrappedReceiver = this.wrapperFactory.CreateWrappedReceiver(receiver, ifc);

                    this.receivers = this.receivers.Add(wrappedReceiver);
                }
            }
        }

        public void DeregisterAll(IReceiver receiver)
        {
            this.receivers = this.receivers.RemoveAll(x => ReferenceEquals(x.WrappedObject, receiver));
        }


        public async Task SendMessageAsync<T>(T message)
        {
            foreach (ReceiverBase<T> receiver in this.receivers.OfType<ReceiverBase<T>>())
            {
                await receiver.ReceiveAsync(message);
            }
        }





    }
}