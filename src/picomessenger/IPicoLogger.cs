﻿#region File Header
// Copyright (c) 2024 Stefan Stolz
#endregion

using System;
using System.Diagnostics.CodeAnalysis;

namespace picomessenger;

public interface IPicoLogger
{
    void ReportException(Exception exception, IReceiver receiver);
    void ReportMessageBlockedToDisabledReceiver(IReceiver receiver);
    void ReportDisablingReceiver(IReceiver receiver);
}

[ExcludeFromCodeCoverage]
public class NullPicoLogger : IPicoLogger
{
    private NullPicoLogger()
    { }

    public static IPicoLogger Instance { get; } = new NullPicoLogger();

    public void ReportException(Exception exception, IReceiver receiver)
    { }

    public void ReportMessageBlockedToDisabledReceiver(IReceiver receiver)
    { }

    public void ReportDisablingReceiver(IReceiver receiver)
    { }
}