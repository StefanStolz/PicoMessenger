namespace picomessenger;

/// <summary>
///     Classes implementing this interface are receiving Messages of the Type <typeparamref name="T" />
/// </summary>
/// <typeparam name="T">The Type of Message that the class receives</typeparam>
public interface IReceiver<in T> : IReceiver
{
    void Receive(T message);
}

/// <summary>
///     Marker Interface for classes that are able to receive Messages
/// </summary>
public interface IReceiver
{ }