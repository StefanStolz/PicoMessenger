using NSubstitute.ExceptionExtensions;

namespace picomessenger.tests
{
    [TestFixture]
    public class ExplorationTests
    {
        public interface ISome
        {
            Task ExecuteAsync();
        }

        [Test]
        public async Task WhenAllWithFaultedTask()
        {
            var fake1 = Substitute.For<ISome>();
            var fake2 = Substitute.For<ISome>();
            var fake3 = Substitute.For<ISome>();

            fake2.ExecuteAsync().Returns(Task.FromException(new Exception()));

            Assert.ThrowsAsync<Exception>(async () =>
            {
                await Task.WhenAll(new ISome[] {fake1, fake2, fake3}.Select(f => f.ExecuteAsync()));
            });

            await fake1.Received(1).ExecuteAsync();
            await fake3.Received(1).ExecuteAsync();
        }


        [Test]
        public async Task WhenAllWithSyncException()
        {
            var fake1 = Substitute.For<ISome>();
            var fake2 = Substitute.For<ISome>();
            var fake3 = Substitute.For<ISome>();

#pragma warning disable NS5003 // Thows is intended
            fake2.ExecuteAsync().Throws(new Exception());
#pragma warning restore NS5003

            Assert.ThrowsAsync<Exception>(async () =>
            {
                await Task.WhenAll(new ISome[] {fake1, fake2, fake3}.Select(f => f.ExecuteAsync()));
            });


            await fake1.Received(1).ExecuteAsync();
            await fake3.DidNotReceive().ExecuteAsync();
        }
        
        [Test]
        public async Task WhenAllWithAsyncException()
        {
            var fake1 = Substitute.For<ISome>();
            var fake2 = Substitute.For<ISome>();
            var fake3 = Substitute.For<ISome>();

            fake2.ExecuteAsync().ThrowsAsync(new Exception());

            Assert.ThrowsAsync<Exception>(async () =>
            {
                await Task.WhenAll(new ISome[] {fake1, fake2, fake3}.Select(f => f.ExecuteAsync()));
            });


            await fake1.Received(1).ExecuteAsync();
            await fake3.Received(1).ExecuteAsync();
        }
    }
}