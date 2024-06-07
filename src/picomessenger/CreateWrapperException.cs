using System;

namespace picomessenger
{
    public class CreateWrapperException : Exception
    {
        public CreateWrapperException(Type wrapperType)
            : base($"Could not create Wrapper for {wrapperType}")
        { }
    }
}