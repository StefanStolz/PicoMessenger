using NSubstitute.ExceptionExtensions;
using picomessenger.wrapper;

namespace picomessenger.tests
{
    [TestFixture]
    public class RegisterTests
    {
        [Test]
        public void RegisterSimpleReceiver()
        {
            var sut = new PicoMessenger();

            var receiverMock = Substitute.For<IReceiver<object>>();

            sut.Register(receiverMock);

            Assert.That(sut.NumberOfRegisteredReceivers, Is.EqualTo(1));
        }

        [Test]
        public async Task SendMessageToReceiver()
        {
            var sut = new PicoMessenger();

            var receiverMock = Substitute.For<IReceiver<object>>();

            sut.Register(receiverMock);

            var message = new object();

            await sut.SendMessageAsync(message);

            receiverMock.Received(1).Receive(message);
        }

        [Test]
        public async Task SendMessageAsyncToReceiver()
        {
            var sut = new PicoMessenger();

            var receiverMock = Substitute.For<IAsyncReceiver<object>>();

            sut.Register(receiverMock);

            var message = new object();

            await sut.SendMessageAsync(message);

            await receiverMock.Received(1).ReceiveAsync(message);
        }

        [Test]
        public async Task NothingReceivedAfterDeregisterReceiver()
        {
            var sut = new PicoMessenger();

            var receiverMock = Substitute.For<IReceiver<object>>();

            sut.Register(receiverMock);
            sut.Deregister(receiverMock);

            var message = new object();

            await sut.SendMessageAsync(message);

            receiverMock.DidNotReceive().Receive(message);
        }

        [Test]
        public async Task NothingReceivedAfterDeregisterAsyncReceiver()
        {
            var sut = new PicoMessenger();

            var receiverMock = Substitute.For<IAsyncReceiver<object>>();

            sut.Register(receiverMock);
            sut.Deregister(receiverMock);

            var message = new object();

            await sut.SendMessageAsync(message);

            await receiverMock.DidNotReceive().ReceiveAsync(message);
        }

        [Test]
        public void ThrowsException()
        {
            var sut = new PicoMessenger();

            var receiverMock = Substitute.For<IAsyncReceiver<object>>();
            receiverMock.ReceiveAsync(Arg.Any<object>()).ThrowsAsync(new Exception());

            sut.Register(receiverMock);

            var message = new object();

            Assert.ThrowsAsync<Exception>(async () => await sut.SendMessageAsync(message));
        }

        [Test]
        public async Task DisableReceiverAfterException()
        {
            var sut = new PicoMessenger(new DisableReceiverOnErrorWrapperFactory());
        
            var receiverMock = Substitute.For<IAsyncReceiver<object>>();
            receiverMock.ReceiveAsync(Arg.Any<object>()).ThrowsAsync(new Exception());
            
            sut.Register(receiverMock);
        
            var message = new object();
        
            await sut.SendMessageAsync(message);
            await sut.SendMessageAsync(message);
        
            await receiverMock.Received(1).ReceiveAsync(message);
        }

        [Test]
        public async Task RegisterAll()
        {
            var sut = new PicoMessenger();

            var receiverMock = Substitute.ForPartsOf<MultiReceiver>();

            sut.RegisterAll(receiverMock);

            await sut.SendMessageAsync(new object());
            await sut.SendMessageAsync("Text");

            Assert.That(sut.NumberOfRegisteredReceivers, Is.EqualTo(2));
            await receiverMock.Received(1).ReceiveAsync(Arg.Any<object>());
            await receiverMock.Received(1).ReceiveAsync(Arg.Any<String>());
        }

        [Test]
        public void DeregisterAll()
        {
            var sut = new PicoMessenger();

            var receiverMock = Substitute.ForPartsOf<MultiReceiver>();

            sut.RegisterAll(receiverMock);
            sut.DeregisterAll(receiverMock);

            Assert.That(sut.NumberOfRegisteredReceivers, Is.EqualTo(0));
        }

        [Test]
        public void DeregisterOneReceiver()
        {
            var sut = new PicoMessenger();

            var receiverMock = Substitute.ForPartsOf<MultiReceiver>();

            sut.RegisterAll(receiverMock);

            sut.Deregister<String>(receiverMock);

            Assert.That(sut.NumberOfRegisteredReceivers, Is.EqualTo(1));
        }

        public class MultiReceiver : IAsyncReceiver<object>, IAsyncReceiver<String>
        {
            public virtual Task ReceiveAsync(object message)
            {
                return Task.CompletedTask;
            }

            public virtual Task ReceiveAsync(string message)
            {
                return Task.CompletedTask;
            }
        }
    }
}