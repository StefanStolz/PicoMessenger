#region File Header
// Copyright (c) 2024 Stefan Stolz
#endregion

using System;

namespace picomessenger;

public class CreateWrapperException : Exception
{
    public CreateWrapperException(Type wrapperType)
        : base($"Could not create Wrapper for {wrapperType}")
    { }
}