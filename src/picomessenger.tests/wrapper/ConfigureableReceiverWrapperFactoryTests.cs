using picomessenger.wrapper;

namespace picomessenger.tests.wrapper
{
    [TestFixture]
    public class ConfigureableReceiverWrapperFactoryTests
    {
        [Test]
        [TestCase(typeof(IAsyncReceiver<string>))]
        [TestCase(typeof(IReceiver<string>))]
        public void CreateWrapper(Type receiverType)
        {
            var sut = new ConfigureableReceiverWrapperFactory(false, false, NullPicoLogger.Instance);

            var receiver = Substitute.For([receiverType], Array.Empty<object>());

            var result = sut.CreateWrappedReceiver(receiver, receiverType);
            
            Assert.That(result, Is.Not.Null);
        }
    }
}