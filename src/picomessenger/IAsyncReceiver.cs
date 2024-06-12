#region File Header
// Copyright (c) 2024 Stefan Stolz
#endregion

using System.Threading.Tasks;

namespace picomessenger;

/// <summary>
///     Classes implementing this interface are receiving Messages of the Type <typeparamref name="T" />
/// </summary>
/// <typeparam name="T">The Type of Message that the class receives</typeparam>
public interface IAsyncReceiver<in T> : IReceiver
{
    Task ReceiveAsync(T message);
}