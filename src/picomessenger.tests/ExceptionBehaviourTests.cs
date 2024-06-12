using NSubstitute.ExceptionExtensions;
using picomessenger.wrapper;

namespace picomessenger.tests;

[TestFixture]
public class ExceptionBehaviourTests
{
    [Test]
    public void MessagesAreSentToAllReceiversEvenTheresAnError()
    {
            PicoMessenger sut = new PicoMessenger(new SimpleReceiverWrapperFactory());

            IReceiver<string>? receiver1 = Substitute.For<IReceiver<string>>();
            IAsyncReceiver<string>? receiver2 = Substitute.For<IAsyncReceiver<string>>();
            // intended to have the exception thrown synchronous
#pragma warning disable NS5003
            receiver2.ReceiveAsync(Arg.Any<string>()).Throws(new Exception());
#pragma warning restore NS5003
            IReceiver<string>? receiver3 = Substitute.For<IReceiver<string>>();

            sut.RegisterSubscriber(receiver1);
            sut.RegisterSubscriber(receiver2);
            sut.RegisterSubscriber(receiver3);

            Assert.ThrowsAsync<Exception>(async () => { await sut.PublishMessageAsync("Text"); });

            receiver1.Received(1).Receive("Text");
            receiver3.Received(1).Receive("Text");
        }


    [Test]
    public void ExceptionIfMultipleErrors()
    {
            PicoMessenger sut = new PicoMessenger(new SimpleReceiverWrapperFactory());

            IReceiver<string>? receiver1 = Substitute.For<IReceiver<string>>();
            IAsyncReceiver<string>? receiver2 = Substitute.For<IAsyncReceiver<string>>();
            receiver2.ReceiveAsync(Arg.Any<string>()).ThrowsAsync(new ArgumentException("Wrong args"));
            IAsyncReceiver<string>? receiver3 = Substitute.For<IAsyncReceiver<string>>();
            receiver3.ReceiveAsync(Arg.Any<string>()).ThrowsAsync(new InvalidOperationException("not valid"));
            IAsyncReceiver<string>? receiver4 = Substitute.For<IAsyncReceiver<string>>();

            sut.RegisterSubscriber(receiver1);
            sut.RegisterSubscriber(receiver2);
            sut.RegisterSubscriber(receiver3);
            sut.RegisterSubscriber(receiver4);

            // Why not an AggregateException?
            ArgumentException? exception = Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await sut.PublishMessageAsync("Text");
            });

            Assert.That(exception!.Message, Is.EqualTo("Wrong args"));

            receiver4.Received(1).ReceiveAsync("Text");
        }
}