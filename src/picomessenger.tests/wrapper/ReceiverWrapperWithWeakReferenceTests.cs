﻿#region File Header
// Copyright (c) 2024 Stefan Stolz
#endregion

using picomessenger.wrapper;

namespace picomessenger.tests.wrapper;

[TestFixture]
public class ReceiverWrapperWithWeakReferenceTests
{
    [Test]
    public async Task SendMessageToWeakReceiver()
    {
            ConfigurableReceiverWrapperFactory sut =
                new ConfigurableReceiverWrapperFactory(true, false, NullPicoLogger.Instance);

            IReceiver<object>? receiver = Substitute.For<IReceiver<object>>();
            IWrappedReceiver<object> wrappedReceiver =
                sut.CreateWrappedReceiver(receiver, typeof(IReceiver<object>)) as IWrappedReceiver<object> ??
                throw new AssertionException(
                    $"Wrong Type - an instance of {typeof(IWrappedReceiver<object>)} expected");

            Assert.That(wrappedReceiver.IsAlive, Is.True);
            Assert.That(wrappedReceiver.WrappedObject, Is.SameAs(receiver));

            await wrappedReceiver.ReceiveAsync(new object());

            receiver.Received(1).Receive(Arg.Any<object>());
        }

    // [Test]
    // [CancelAfter(60_000)]
    // public async Task SendMessageToCollectedReceiver(CancellationToken cancellationToken)
    // {
    //     var sut = new WeakWrapperFactory();
    //
    //     var receiver = Substitute.For<IReceiver<object>>();
    //     var wrappedReceiver =
    //         (sut.CreateWrappedReceiver(receiver, typeof(IReceiver<object>)) as IWrappedReceiver<object>) ??
    //         throw new Exception();
    //
    //     receiver = null;
    //
    //     while (wrappedReceiver.IsAlive)
    //     {
    //         cancellationToken.ThrowIfCancellationRequested();
    //         GC.Collect();
    //         GC.WaitForFullGCComplete();
    //         GC.WaitForPendingFinalizers();
    //         GC.Collect();
    //         Thread.Sleep(50);
    //     }
    //
    //     Assert.That(wrappedReceiver.IsAlive, Is.False);
    // }


    [Test]
    public async Task SendMessageToAsyncWeakReceiver()
    {
            ConfigurableReceiverWrapperFactory sut =
                new ConfigurableReceiverWrapperFactory(true, false, NullPicoLogger.Instance);

            IAsyncReceiver<object>? receiver = Substitute.For<IAsyncReceiver<object>>();
            IWrappedReceiver<object> wrappedReceiver =
                sut.CreateWrappedReceiver(receiver, typeof(IAsyncReceiver<object>)) as IWrappedReceiver<object> ??
                throw new AssertionException(
                    $"Wrong Type - an instance of {typeof(IWrappedReceiver<object>)} expected");

            Assert.That(wrappedReceiver, Is.Not.Null);
            Assert.That(wrappedReceiver.IsAlive, Is.True);
            Assert.That(wrappedReceiver.WrappedObject, Is.SameAs(receiver));

            await wrappedReceiver.ReceiveAsync(new object());

            await receiver.Received(1).ReceiveAsync(Arg.Any<object>());
        }
}