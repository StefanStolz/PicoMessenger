using System;

namespace picomessenger
{
    public interface IWrappedReceiver
    {
        IReceiver WrappedObject { get; }

        Type MessageType { get; }
    }
}