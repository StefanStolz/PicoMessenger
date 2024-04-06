using NSubstitute.ExceptionExtensions;
using picomessenger.wrapper;

namespace picomessenger.tests
{
    [TestFixture]
    public class ExceptionBehaviourTests
    {
        [Test]
        public void MessagesAreSentToAllReceiversEvenTheresAnError()
        {
            var sut = new PicoMessenger(new SimpleReceiverWrapperFactory());

            var receiver1 = Substitute.For<IReceiver<string>>();
            var receiver2 = Substitute.For<IAsyncReceiver<string>>();
            // intended to have the exception thrown synchronous
#pragma warning disable NS5003
            receiver2.ReceiveAsync(Arg.Any<string>()).Throws(new Exception());
#pragma warning restore NS5003
            var receiver3 = Substitute.For<IReceiver<string>>();

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
            var sut = new PicoMessenger(new SimpleReceiverWrapperFactory());

            var receiver1 = Substitute.For<IReceiver<string>>();
            var receiver2 = Substitute.For<IAsyncReceiver<string>>();
            receiver2.ReceiveAsync(Arg.Any<string>()).ThrowsAsync(new ArgumentException("Wrong args"));
            var receiver3 = Substitute.For<IAsyncReceiver<string>>();
            receiver3.ReceiveAsync(Arg.Any<string>()).ThrowsAsync(new InvalidOperationException("not valid"));
            var receiver4 = Substitute.For<IAsyncReceiver<string>>();
            
            sut.RegisterSubscriber(receiver1);
            sut.RegisterSubscriber(receiver2);
            sut.RegisterSubscriber(receiver3);
            sut.RegisterSubscriber(receiver4);

            // Why not an AggregateException?
            var exception = Assert.ThrowsAsync<ArgumentException>(async () => { await sut.PublishMessageAsync("Text"); });
            
             Assert.That(exception!.Message, Is.EqualTo("Wrong args"));

            receiver4.Received(1).ReceiveAsync("Text");
        }
    }
}