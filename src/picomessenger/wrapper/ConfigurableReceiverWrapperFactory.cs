﻿#region File Header
// Copyright (c) 2024 Stefan Stolz
#endregion

using System;
using System.Threading.Tasks;

namespace picomessenger.wrapper;

internal class ConfigurableReceiverWrapperFactory : IReceiverWrapperFactory
{
    public ConfigurableReceiverWrapperFactory(bool useWeakReferences, bool disableOnError, IPicoLogger logger)
    {
            this.UseWeakReferences = useWeakReferences;
            this.DisableOnError = disableOnError;
            this.Logger = logger;
        }

    internal bool UseWeakReferences { get; }

    internal bool DisableOnError { get; }

    internal IPicoLogger Logger { get; }

    public IWrappedReceiver CreateWrappedReceiver(object receiver, Type receiverInterfaceType)
    {
            if (receiverInterfaceType.IsGenericType)
            {
                object? configuredReceiver = null;
                Type[] genericArguments = receiverInterfaceType.GetGenericArguments();

                if (receiverInterfaceType.GetGenericTypeDefinition() == typeof(IAsyncReceiver<>))
                {
                    if (this.UseWeakReferences)
                    {
                        Type r = typeof(WeakAsyncReceiver<>).MakeGenericType(genericArguments);
                        configuredReceiver = Activator.CreateInstance(r, receiver);
                    }
                    else
                    {
                        Type r = typeof(AsyncConfiguredReceiver<>).MakeGenericType(genericArguments);
                        configuredReceiver = Activator.CreateInstance(r, receiver);
                    }
                }

                if (receiverInterfaceType.GetGenericTypeDefinition() == typeof(IReceiver<>))
                {
                    if (this.UseWeakReferences)
                    {
                        Type r = typeof(WeakSyncConfiguredReceiver<>).MakeGenericType(genericArguments);
                        configuredReceiver = Activator.CreateInstance(r, receiver);
                    }
                    else
                    {
                        Type r = typeof(SyncConfiguredReceiver<>).MakeGenericType(genericArguments);
                        configuredReceiver = Activator.CreateInstance(r, receiver);
                    }
                }

                if (configuredReceiver != null)
                {
                    Type t = typeof(ConfigureableWrappedReceiverBase<>).MakeGenericType(genericArguments);

                    return (IWrappedReceiver)(Activator.CreateInstance(t, this.Logger, this.DisableOnError,
                        configuredReceiver) ?? throw new CreateWrapperException(t));
                }
            }

            throw new ArgumentException("Could not wrap Receiver");
        }

    private interface IConfiguredReceiver<T>
    {
        bool IsAlive { get; }

        IReceiver? WrappedObject { get; }
        Task SendMessageAsync(T message);
    }

    private class AsyncConfiguredReceiver<T> : IConfiguredReceiver<T>
    {
        private readonly IAsyncReceiver<T> receiver;

        public AsyncConfiguredReceiver(IAsyncReceiver<T> receiver)
        {
                this.receiver = receiver;
            }

        public Task SendMessageAsync(T message) => this.receiver.ReceiveAsync(message);

        public bool IsAlive => true;
        public IReceiver? WrappedObject => this.receiver;
    }

    private class WeakAsyncReceiver<T> : IConfiguredReceiver<T>
    {
        private readonly WeakReference<IAsyncReceiver<T>> weakReceiver;

        public WeakAsyncReceiver(IAsyncReceiver<T> receiver)
        {
                this.weakReceiver = new WeakReference<IAsyncReceiver<T>>(receiver);
            }

        public Task SendMessageAsync(T message)
        {
                if (this.weakReceiver.TryGetTarget(out IAsyncReceiver<T>? receiver))
                {
                    return receiver.ReceiveAsync(message);
                }

                return Task.CompletedTask;
            }

        public bool IsAlive => this.weakReceiver.TryGetTarget(out _);

        public IReceiver? WrappedObject
        {
            get
            {
                    if (this.weakReceiver.TryGetTarget(out IAsyncReceiver<T>? receiver))
                    {
                        return receiver;
                    }

                    return null;
                }
        }
    }

    private sealed class SyncConfiguredReceiver<T> : IConfiguredReceiver<T>
    {
        private readonly IReceiver<T> receiver;

        public SyncConfiguredReceiver(IReceiver<T> receiver)
        {
                this.receiver = receiver;
            }

        public Task SendMessageAsync(T message)
        {
                this.receiver.Receive(message);
                return Task.CompletedTask;
            }

        public bool IsAlive => true;
        public IReceiver WrappedObject => this.receiver;
    }

    private sealed class WeakSyncConfiguredReceiver<T> : IConfiguredReceiver<T>
    {
        private readonly WeakReference<IReceiver<T>> weakReceiver;

        public WeakSyncConfiguredReceiver(IReceiver<T> receiver)
        {
                this.weakReceiver = new WeakReference<IReceiver<T>>(receiver);
            }

        public Task SendMessageAsync(T message)
        {
                if (this.weakReceiver.TryGetTarget(out IReceiver<T>? receiver))
                {
                    receiver.Receive(message);
                }

                return Task.CompletedTask;
            }

        public bool IsAlive => this.weakReceiver.TryGetTarget(out _);

        public IReceiver? WrappedObject
        {
            get
            {
                    if (this.weakReceiver.TryGetTarget(out IReceiver<T>? receiver))
                    {
                        return receiver;
                    }

                    return null;
                }
        }
    }

    private sealed class ConfigureableWrappedReceiverBase<T> : WrappedReceiverBase<T>
    {
        private readonly bool disableOnError;
        private readonly IPicoLogger logger;
        private readonly IConfiguredReceiver<T> receiver;

        private bool isDisabledByError;

        public ConfigureableWrappedReceiverBase(IPicoLogger logger, bool disableOnError,
            IConfiguredReceiver<T> receiver)
        {
                this.logger = logger;
                this.disableOnError = disableOnError;
                this.receiver = receiver;
            }

        public override bool IsAlive => this.receiver.IsAlive && !this.isDisabledByError;

        public override IReceiver? WrappedObject => this.receiver.WrappedObject;

        protected override async Task SendMessageAsync(T message)
        {
                if (this.isDisabledByError && this.disableOnError)
                {
                    if (this.receiver.WrappedObject is not null)
                    {
                        this.logger.ReportMessageBlockedToDisabledReceiver(this.receiver.WrappedObject);
                    }

                    return;
                }

                try
                {
                    await this.receiver.SendMessageAsync(message);
                }
                catch (Exception exception)
                {
                    if (this.disableOnError)
                    {
                        this.isDisabledByError = true;
                        if (this.receiver.WrappedObject is not null)
                        {
                            this.logger.ReportDisablingReceiver(this.receiver.WrappedObject);
                        }
                    }

                    if (this.receiver.WrappedObject is not null)
                    {
                        this.logger.ReportException(exception, this.receiver.WrappedObject);
                    }
                }
            }
    }
}