using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using picomessenger.wrapper;

namespace picomessenger;

public class PicoMessenger : IMessenger
{
    private readonly IReceiverWrapperFactory defaultWrapperFactory;
    private ImmutableArray<IWrappedReceiver> receivers = ImmutableArray.Create<IWrappedReceiver>();

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
    public void RegisterSubscriber<T>(IReceiver<T> receiver, IReceiverWrapperFactory wrapperFactory)
    {
        if (receiver == null)
        {
            throw new ArgumentNullException(nameof(receiver));
        }

        if (wrapperFactory == null)
        {
            throw new ArgumentNullException(nameof(wrapperFactory));
        }


        this.RegisterSingle(receiver, wrapperFactory);
    }

    /// <inheritdoc />
    public void RegisterSubscriber<T>(IReceiver<T> receiver)
    {
        if (receiver == null)
        {
            throw new ArgumentNullException(nameof(receiver));
        }

        this.RegisterSingle(receiver, this.defaultWrapperFactory);
    }

    /// <inheritdoc />
    public void RegisterSubscriber<T>(IAsyncReceiver<T> receiver, IReceiverWrapperFactory wrapperFactory)
    {
        if (receiver == null)
        {
            throw new ArgumentNullException(nameof(receiver));
        }

        if (wrapperFactory == null)
        {
            throw new ArgumentNullException(nameof(wrapperFactory));
        }

        this.RegisterSingle(receiver, wrapperFactory);
    }

    /// <inheritdoc />
    public void RegisterSubscriber<T>(IAsyncReceiver<T> receiver)
    {
        if (receiver == null)
        {
            throw new ArgumentNullException(nameof(receiver));
        }

        this.RegisterSingle(receiver, this.defaultWrapperFactory);
    }

    /// <inheritdoc />
    public void UnregisterSubscriber<T>(IReceiver<T> receiver)
    {
        if (receiver == null)
        {
            throw new ArgumentNullException(nameof(receiver));
        }

        this.receivers = this.receivers.RemoveAll(x => ReferenceEquals(x.WrappedObject, receiver) &&
                                                       x.MessageType == typeof(T));
    }

    /// <inheritdoc />
    public void UnregisterSubscriber<T>(IAsyncReceiver<T> receiver)
    {
        if (receiver == null)
        {
            throw new ArgumentNullException(nameof(receiver));
        }

        this.receivers = this.receivers.RemoveAll(x => ReferenceEquals(x.WrappedObject, receiver) &&
                                                       x.MessageType == typeof(T));
    }

    /// <inheritdoc />
    public void RegisterAll(IReceiver receiver)
    {
        if (receiver == null)
        {
            throw new ArgumentNullException(nameof(receiver));
        }

        this.RegisterAll(receiver, this.defaultWrapperFactory);
    }

    /// <inheritdoc />
    public void RegisterAll(IReceiver receiver, IReceiverWrapperFactory wrapperFactory)
    {
        if (receiver == null)
        {
            throw new ArgumentNullException(nameof(receiver));
        }

        if (wrapperFactory == null)
        {
            throw new ArgumentNullException(nameof(wrapperFactory));
        }

        Type[] interfaces = receiver.GetType().GetInterfaces();

        foreach (Type ifc in interfaces)
        {
            if (ifc.IsGenericType && typeof(IReceiver).IsAssignableFrom(ifc))
            {
                IWrappedReceiver wrappedReceiver = wrapperFactory.CreateWrappedReceiver(receiver, ifc);

                this.receivers = this.receivers.Add(wrappedReceiver);
            }
        }
    }

    /// <inheritdoc />
    public void DeregisterAll(IReceiver receiver) =>
        this.receivers = this.receivers.RemoveAll(x => ReferenceEquals(x.WrappedObject, receiver));

    /// <inheritdoc />
    public async Task PublishMessageAsync<T>(T message) =>
        await Task.WhenAll(
            this.receivers.Where(r => r.IsAlive)
                .OfType<IWrappedReceiver<T>>()
                .Select(r => r.ReceiveAsync(message)));

    private void RegisterSingle<T>(T receiver, IReceiverWrapperFactory wrapperFactory) where T : IReceiver
    {
        Type receiverInterfaceType = typeof(T);
        IWrappedReceiver wrappedReceiver = wrapperFactory.CreateWrappedReceiver(receiver, receiverInterfaceType);

        this.receivers = this.receivers.Add(wrappedReceiver);
    }
}