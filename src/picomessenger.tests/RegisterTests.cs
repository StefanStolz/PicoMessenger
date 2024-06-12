using NSubstitute.ExceptionExtensions;
using picomessenger.wrapper;

namespace picomessenger.tests;

[TestFixture]
public class RegisterTests
{
    [Test]
    public void RegisterSimpleReceiver()
    {
            PicoMessenger sut = new PicoMessenger();

            IReceiver<object>? receiverMock = Substitute.For<IReceiver<object>>();

            sut.RegisterSubscriber(receiverMock);

            Assert.That(sut.NumberOfRegisteredReceivers, Is.EqualTo(1));
        }

    [Test]
    public async Task SendMessageToReceiver()
    {
            PicoMessenger sut = new PicoMessenger();

            IReceiver<object>? receiverMock = Substitute.For<IReceiver<object>>();

            sut.RegisterSubscriber(receiverMock);

            object message = new object();

            await sut.PublishMessageAsync(message);

            receiverMock.Received(1).Receive(message);
        }

    [Test]
    public async Task SendMessageAsyncToReceiver()
    {
            PicoMessenger sut = new PicoMessenger();

            IAsyncReceiver<object>? receiverMock = Substitute.For<IAsyncReceiver<object>>();

            sut.RegisterSubscriber(receiverMock);

            object message = new object();

            await sut.PublishMessageAsync(message);

            await receiverMock.Received(1).ReceiveAsync(message);
        }

    [Test]
    public async Task NothingReceivedAfterDeregisterReceiver()
    {
            PicoMessenger sut = new PicoMessenger();

            IReceiver<object>? receiverMock = Substitute.For<IReceiver<object>>();

            sut.RegisterSubscriber(receiverMock);
            sut.UnregisterSubscriber(receiverMock);

            object message = new object();

            await sut.PublishMessageAsync(message);

            receiverMock.DidNotReceive().Receive(message);
        }

    [Test]
    public async Task NothingReceivedAfterDeregisterAsyncReceiver()
    {
            PicoMessenger sut = new PicoMessenger();

            IAsyncReceiver<object>? receiverMock = Substitute.For<IAsyncReceiver<object>>();

            sut.RegisterSubscriber(receiverMock);
            sut.UnregisterSubscriber(receiverMock);

            object message = new object();

            await sut.PublishMessageAsync(message);

            await receiverMock.DidNotReceive().ReceiveAsync(message);
        }

    [Test]
    public void ThrowsException()
    {
            PicoMessenger sut = new PicoMessenger();

            IAsyncReceiver<object>? receiverMock = Substitute.For<IAsyncReceiver<object>>();
            receiverMock.ReceiveAsync(Arg.Any<object>()).ThrowsAsync(new Exception());

            sut.RegisterSubscriber(receiverMock);

            object message = new object();

            Assert.ThrowsAsync<Exception>(async () => await sut.PublishMessageAsync(message));
        }

    [Test]
    public async Task DisableReceiverAfterException()
    {
            PicoMessenger sut =
                new PicoMessenger(new ConfigurableReceiverWrapperFactory(false, true, NullPicoLogger.Instance));

            IAsyncReceiver<object>? receiverMock = Substitute.For<IAsyncReceiver<object>>();
            receiverMock.ReceiveAsync(Arg.Any<object>()).ThrowsAsync(new Exception());

            sut.RegisterSubscriber(receiverMock);

            object message = new object();

            await sut.PublishMessageAsync(message);
            await sut.PublishMessageAsync(message);

            await receiverMock.Received(1).ReceiveAsync(message);
        }

    [Test]
    public async Task RegisterAll()
    {
            PicoMessenger sut = new PicoMessenger();

            MultiReceiver? receiverMock = Substitute.ForPartsOf<MultiReceiver>();

            sut.RegisterAll(receiverMock);

            await sut.PublishMessageAsync(new object());
            await sut.PublishMessageAsync("Text");

            Assert.That(sut.NumberOfRegisteredReceivers, Is.EqualTo(2));
            await receiverMock.Received(1).ReceiveAsync(Arg.Any<object>());
            await receiverMock.Received(1).ReceiveAsync(Arg.Any<String>());
        }

    [Test]
    public void DeregisterAll()
    {
            PicoMessenger sut = new PicoMessenger();

            MultiReceiver? receiverMock = Substitute.ForPartsOf<MultiReceiver>();

            sut.RegisterAll(receiverMock);
            sut.DeregisterAll(receiverMock);

            Assert.That(sut.NumberOfRegisteredReceivers, Is.EqualTo(0));
        }

    [Test]
    public void DeregisterOneReceiver()
    {
            PicoMessenger sut = new PicoMessenger();

            MultiReceiver? receiverMock = Substitute.ForPartsOf<MultiReceiver>();

            sut.RegisterAll(receiverMock);

            sut.UnregisterSubscriber<String>(receiverMock);

            Assert.That(sut.NumberOfRegisteredReceivers, Is.EqualTo(1));
        }


    [Test]
    public void RegisterAsyncReceiverWithCustomWrapperFactory()
    {
            IReceiverWrapperFactory? defaultWrapperFactory = Substitute.For<IReceiverWrapperFactory>();
            PicoMessenger sut = new PicoMessenger(defaultWrapperFactory);

            IAsyncReceiver<string>? receiver = Substitute.For<IAsyncReceiver<string>>();

            IReceiverWrapperFactory? customWrapperFactory = Substitute.For<IReceiverWrapperFactory>();

            sut.RegisterSubscriber(receiver, customWrapperFactory);

            defaultWrapperFactory.DidNotReceive().CreateWrappedReceiver(Arg.Any<object>(), Arg.Any<Type>());
            customWrapperFactory.Received(1).CreateWrappedReceiver(receiver, Arg.Any<Type>());
        }

    [Test]
    public void RegisterReceiverWithCustomWrapperFactory()
    {
            IReceiverWrapperFactory? defaultWrapperFactory = Substitute.For<IReceiverWrapperFactory>();
            PicoMessenger sut = new PicoMessenger(defaultWrapperFactory);

            IReceiver<string>? receiver = Substitute.For<IReceiver<string>>();

            IReceiverWrapperFactory? customWrapperFactory = Substitute.For<IReceiverWrapperFactory>();

            sut.RegisterSubscriber(receiver, customWrapperFactory);

            defaultWrapperFactory.DidNotReceive().CreateWrappedReceiver(Arg.Any<object>(), Arg.Any<Type>());
            customWrapperFactory.Received(1).CreateWrappedReceiver(receiver, Arg.Any<Type>());
        }


    [Test]
    public void RegisterallWithCustomWrapperFactory()
    {
            IReceiverWrapperFactory? defaultWrapperFactory = Substitute.For<IReceiverWrapperFactory>();
            PicoMessenger sut = new PicoMessenger(defaultWrapperFactory);

            MultiReceiver receiver = new MultiReceiver();

            IReceiverWrapperFactory? customWrapperFactory = Substitute.For<IReceiverWrapperFactory>();

            sut.RegisterAll(receiver, customWrapperFactory);

            defaultWrapperFactory.DidNotReceive().CreateWrappedReceiver(Arg.Any<object>(), Arg.Any<Type>());
            customWrapperFactory.Received(2).CreateWrappedReceiver(receiver, Arg.Any<Type>());
        }

    public class MultiReceiver : IAsyncReceiver<object>, IAsyncReceiver<String>
    {
        public virtual Task ReceiveAsync(object message) => Task.CompletedTask;

        public virtual Task ReceiveAsync(string message) => Task.CompletedTask;
    }
}