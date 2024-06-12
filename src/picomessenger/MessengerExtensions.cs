using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace picomessenger;

public static class MessengerExtensions
{
    public static IRegistry Register<T>(this IMessageSubscriberRegistry messenger, Action<T> target)
    {
        Registry registry = new Registry(messenger);

        registry.Add(target);

        return registry;
    }

    public static IRegistry Register<T>(this IMessageSubscriberRegistry messenger, Func<T, Task> target)
    {
        Registry registry = new Registry(messenger);

        registry.Add(target);

        return registry;
    }

    public static IRegistry And<T>(this IRegistry registry, Action<T> target)
    {
        Registry r = (Registry)registry;

        r.Add(target);

        return registry;
    }

    public static IRegistry And<T>(this IRegistry registry, Func<T, Task> target)
    {
        Registry r = (Registry)registry;

        r.Add(target);

        return registry;
    }
}

internal class GenericReceiver<T> : IReceiver<T>
{
    private readonly Action<T> target;

    public GenericReceiver(Action<T> target)
    {
        this.target = target;
    }

    public void Receive(T message) => this.target(message);
}

internal class Registry : IRegistry
{
    private readonly IMessageSubscriberRegistry messenger;

    private readonly List<IReceiver> receivers = new();

    public Registry(IMessageSubscriberRegistry messenger)
    {
        this.messenger = messenger;
    }

    public void Dispose()
    {
        foreach (IReceiver? receiver in this.receivers)
        {
            this.messenger.DeregisterAll(receiver);
        }
    }

    public void Add<T>(Action<T> target)
    {
        GenericReceiver<T> r = new GenericReceiver<T>(target);
        this.receivers.Add(r);
        this.messenger.RegisterSubscriber(r);
    }

    public void Add<T>(Func<T, Task> target)
    {
        GenericAsyncReceiver<T> r = new GenericAsyncReceiver<T>(target);
        this.receivers.Add(r);
        this.messenger.RegisterSubscriber(r);
    }
}

internal class GenericAsyncReceiver<T> : IAsyncReceiver<T>
{
    private readonly Func<T, Task> target;

    public GenericAsyncReceiver(Func<T, Task> target)
    {
        this.target = target;
    }

    public Task ReceiveAsync(T message) => this.target(message);
}

public interface IRegistry : IDisposable
{ }