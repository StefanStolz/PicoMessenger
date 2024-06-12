#region File Header
// Copyright (c) 2024 Stefan Stolz
#endregion

using NSubstitute.ExceptionExtensions;

namespace picomessenger.tests;

[TestFixture]
public class ExplorationTests
{
    // ReSharper disable once MemberCanBePrivate.Global
    public interface ISome
    {
        Task ExecuteAsync();
    }

    [Test]
    public async Task WhenAllWithFaultedTask()
    {
            ISome? fake1 = Substitute.For<ISome>();
            ISome? fake2 = Substitute.For<ISome>();
            ISome? fake3 = Substitute.For<ISome>();

            fake2.ExecuteAsync().Returns(Task.FromException(new Exception()));

            Assert.ThrowsAsync<Exception>(async () =>
            {
                await Task.WhenAll(new[] { fake1, fake2, fake3 }.Select(f => f.ExecuteAsync()));
            });

            await fake1.Received(1).ExecuteAsync();
            await fake3.Received(1).ExecuteAsync();
        }


    [Test]
    public async Task WhenAllWithSyncException()
    {
            ISome? fake1 = Substitute.For<ISome>();
            ISome? fake2 = Substitute.For<ISome>();
            ISome? fake3 = Substitute.For<ISome>();

#pragma warning disable NS5003 // Thows is intended
            fake2.ExecuteAsync().Throws(new Exception());
#pragma warning restore NS5003

            Assert.ThrowsAsync<Exception>(async () =>
            {
                await Task.WhenAll(new[] { fake1, fake2, fake3 }.Select(f => f.ExecuteAsync()));
            });


            await fake1.Received(1).ExecuteAsync();
            await fake3.DidNotReceive().ExecuteAsync();
        }

    [Test]
    public async Task WhenAllWithAsyncException()
    {
            ISome? fake1 = Substitute.For<ISome>();
            ISome? fake2 = Substitute.For<ISome>();
            ISome? fake3 = Substitute.For<ISome>();

            fake2.ExecuteAsync().ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(async () =>
            {
                await Task.WhenAll(new[] { fake1, fake2, fake3 }.Select(f => f.ExecuteAsync()));
            });

            await fake1.Received(1).ExecuteAsync();
            await fake3.Received(1).ExecuteAsync();
        }

    [Test]
    public void WhenAllWithMultipleFaultedExceptions()
    {
            Task t1 = Task.FromException(new ArgumentException());
            Task t2 = Task.FromException(new InvalidOperationException());

            // Why not an AggregateException?
            Assert.ThrowsAsync<ArgumentException>(async () => { await Task.WhenAll(t1, t2); });
        }

    // [Test]
    // [CancelAfter(10_000)]
    // public void CollectTargetOfWeakReference(CancellationToken cancellationToken)
    // {
    //     var reference = new FakeSome();
    //
    //     var wr = new WeakReference<ISome>(reference);
    //
    //     reference = null;
    //
    //     while (wr.TryGetTarget(out _))
    //     {
    //         GC.AddMemoryPressure(100_000_000);
    //         GC.Collect();
    //         GC.WaitForPendingFinalizers();
    //         GC.WaitForFullGCComplete();
    //         Thread.Sleep(100);
    //         cancellationToken.ThrowIfCancellationRequested();
    //     }
    // }
    //
    // public class FakeSome : ISome
    // {
    //     public Task ExecuteAsync() => Task.CompletedTask;
    // }
}