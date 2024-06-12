// Copyright © Stefan Stolz, 2024

using System;
using picomessenger.wrapper;

namespace picomessenger;

public static class Messengers
{
    /// <summary>
    ///     Returns a Messenger no special Features.
    /// </summary>
    /// <returns>A new instance of <see cref="PicoMessenger" /></returns>
    public static PicoMessenger SimpleMessenger() => new(new SimpleReceiverWrapperFactory());

    /// <summary>
    ///     Returns a Messenger that uses <see cref="WeakReference{T}" /> to hold the Subscribers
    /// </summary>
    /// <returns>A new instance of <see cref="PicoMessenger" /></returns>
    public static PicoMessenger WeakMessenger() =>
        new(new ConfigurableReceiverWrapperFactory(true, false, NullPicoLogger.Instance));
}