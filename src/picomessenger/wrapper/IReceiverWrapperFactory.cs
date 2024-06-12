#region File Header
// Copyright (c) 2024 Stefan Stolz
#endregion

using System;

namespace picomessenger.wrapper;

public interface IReceiverWrapperFactory
{
    IWrappedReceiver CreateWrappedReceiver(object receiver, Type receiverInterfaceType);
}