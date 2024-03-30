using System;

namespace picomessenger
{
    public interface IReceiverWrapperFactory
    {
        IWrappedReceiver CreateWrappedReceiver(object receiver, Type receiverInterfaceType);
    }
}