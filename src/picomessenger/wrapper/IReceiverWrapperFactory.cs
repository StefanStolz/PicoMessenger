using System;

namespace picomessenger.wrapper
{
    public interface IReceiverWrapperFactory
    {
        IWrappedReceiver CreateWrappedReceiver(object receiver, Type receiverInterfaceType);
    }
}