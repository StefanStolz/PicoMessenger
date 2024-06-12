#region File Header
// Copyright (c) 2024 Stefan Stolz
#endregion

using System;
using System.Threading.Tasks;

namespace picomessenger.wrapper;

public abstract class WrappedReceiverBase<T> : IWrappedReceiver<T>
{
    public Task ReceiveAsync(T message)
    {
            try
            {
                return this.SendMessageAsync(message);
            }
            catch (Exception exception)
            {
                return Task.FromException(exception);
            }
        }

    public abstract IReceiver? WrappedObject { get; }

    public abstract bool IsAlive { get; }

    public Type MessageType { get; } = typeof(T);

    protected abstract Task SendMessageAsync(T message);
}