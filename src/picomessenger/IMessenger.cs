﻿#region File Header
// Copyright (c) 2024 Stefan Stolz
#endregion

using System.Threading.Tasks;
using picomessenger.wrapper;

namespace picomessenger;

/// <summary>
///     <see cref="IMessageSubscriberRegistry" /> defines methods to register and deregister Message receivers.
/// </summary>
public interface IMessageSubscriberRegistry
{
    /// <summary>
    ///     Registers a specific Message Receiver.
    /// </summary>
    /// <param name="receiver">An instance of a class receiving <typeparamref name="T" /> Messages</param>
    /// <typeparam name="T">The Type of the Message to receive</typeparam>
    void RegisterSubscriber<T>(IReceiver<T> receiver);

    /// <summary>
    ///     Registers a specific Message Receiver.
    /// </summary>
    /// <param name="receiver">An instance of a class receiving <typeparamref name="T" /> Messages</param>
    /// <typeparam name="T">The Type of the Message to receive</typeparam>
    void RegisterSubscriber<T>(IAsyncReceiver<T> receiver);


    /// <summary>
    ///     Registers a specific Message Receiver with a custom <see cref="IReceiverWrapperFactory" />.
    /// </summary>
    /// <param name="receiver">An instance of a class receiving <typeparamref name="T" /> Messages</param>
    /// <param name="wrapperFactory">The custom <see cref="IReceiverWrapperFactory" /> to use</param>
    /// <typeparam name="T">The Type of the Message to receive</typeparam>
    void RegisterSubscriber<T>(IReceiver<T> receiver, IReceiverWrapperFactory wrapperFactory);

    /// <summary>
    ///     Registers a specific Message Receiver with a custom <see cref="IReceiverWrapperFactory" />.
    /// </summary>
    /// <param name="receiver">An instance of a class receiving <typeparamref name="T" /> Messages</param>
    /// <param name="wrapperFactory">The custom <see cref="IReceiverWrapperFactory" /> to use</param>
    /// <typeparam name="T">The Type of the Message to receive</typeparam>
    void RegisterSubscriber<T>(IAsyncReceiver<T> receiver, IReceiverWrapperFactory wrapperFactory);

    /// <summary>
    ///     Unregisters an existing Registration of a specific message of an instance.
    /// </summary>
    /// <param name="receiver">The instance that should be unregistered</param>
    /// <typeparam name="T">The Type of the Message that should be unregistered</typeparam>
    void UnregisterSubscriber<T>(IReceiver<T> receiver);

    /// <summary>
    ///     Unregisters an existing Registration of a specific message of an instance.
    /// </summary>
    /// <param name="receiver">The instance that should be unregistered</param>
    /// <typeparam name="T">The Type of the Message that should be unregistered</typeparam>
    void UnregisterSubscriber<T>(IAsyncReceiver<T> receiver);

    /// <summary>
    ///     Registers all implemented <see cref="IReceiver{T}" /> and <see cref="IAsyncReceiver{T}" />
    /// </summary>
    /// <param name="receiver">The instance to register</param>
    void RegisterAll(IReceiver receiver);

    /// <summary>
    ///     Registers all implemented <see cref="IReceiver{T}" /> and <see cref="IAsyncReceiver{T}" />
    ///     with a custom <see cref="IReceiverWrapperFactory" />
    /// </summary>
    /// <param name="receiver">The instance to register</param>
    /// <param name="wrapperFactory">The <see cref="IReceiverWrapperFactory" /> to use</param>
    void RegisterAll(IReceiver receiver, IReceiverWrapperFactory wrapperFactory);

    /// <summary>
    ///     Unregisters all implemented <see cref="IReceiver{T}" /> and <see cref="IAsyncReceiver{T}" />
    /// </summary>
    /// <param name="receiver">The instance to unregister</param>
    void DeregisterAll(IReceiver receiver);
}

/// <summary>
///     Defines Methods to send Messages
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    ///     Publishes a Message to all registered Subscribers
    /// </summary>
    /// <param name="message">The Message to publish</param>
    /// <typeparam name="T">The Type of the Message</typeparam>
    /// <returns>A Task to await the delivery of the Message to all receivers</returns>
    Task PublishMessageAsync<T>(T message);
}

/// <summary>
///     Defines Methods of a Messenger
/// </summary>
public interface IMessenger : IMessageSubscriberRegistry, IMessagePublisher
{ }