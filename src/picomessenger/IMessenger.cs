using System.Threading.Tasks;

namespace picomessenger
{
    /// <summary>
    /// <see cref="IMessengerRegistration"/> defines methods to register and deregister Message receivers.
    /// </summary>
    public interface IMessengerRegistration
    {
        /// <summary>
        /// Registers a specific Message Receiver.
        /// </summary>
        /// <param name="receiver">An instance of a class receiving <typeparamref name="T"/> Messages</param>
        /// <typeparam name="T">The Type of the Message to receive</typeparam>
        void Register<T>(IReceiver<T> receiver);
        
        /// <summary>
        /// Registers a specific Message Receiver.
        /// </summary>
        /// <param name="receiver">An instance of a class receiving <typeparamref name="T"/> Messages</param>
        /// <typeparam name="T">The Type of the Message to receive</typeparam>
        void Register<T>(IAsyncReceiver<T> receiver);
        
        /// <summary>
        /// Unregisters an existing Registration of a specific message of an instance.
        /// </summary>
        /// <param name="receiver">The instance that should be unregistered</param>
        /// <typeparam name="T">The Type of the Message that should be unregistered</typeparam>
        void Deregister<T>(IReceiver<T> receiver);
        
        /// <summary>
        /// Unregisters an existing Registration of a specific message of an instance.
        /// </summary>
        /// <param name="receiver">The instance that should be unregistered</param>
        /// <typeparam name="T">The Type of the Message that should be unregistered</typeparam>
        void Deregister<T>(IAsyncReceiver<T> receiver);

        /// <summary>
        /// Registers all implemented <see cref="IReceiver{T}"/> and <see cref="IAsyncReceiver{T}"/>
        /// </summary>
        /// <param name="receiver">The instance to register</param>
        void RegisterAll(IReceiver receiver);
       
        /// <summary>
        /// Unregisters all implemented <see cref="IReceiver{T}"/> and <see cref="IAsyncReceiver{T}"/>
        /// </summary>
        /// <param name="receiver">The instance to unregister</param>
        void DeregisterAll(IReceiver receiver);
    }

    /// <summary>
    /// Defines Methods to send Messages
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        /// Sends a Message to all registered Receivers
        /// </summary>
        /// <param name="message">The Message to send</param>
        /// <typeparam name="T">The Type of the Message</typeparam>
        /// <returns>A Task to await the delivery of the Message to all receivers</returns>
        Task SendMessageAsync<T>(T message);
    }

    /// <summary>
    /// Defines Methods of a Messenger
    /// </summary>
    public interface IMessenger : IMessengerRegistration, IMessageSender
    {
    }
}