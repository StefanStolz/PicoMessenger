using picomessenger.wrapper;

namespace picomessenger.tests.wrapper;

[TestFixture]
public class ConfigurableReceiverWrapperFactoryTests
{
    [Test]
    [TestCase(typeof(IAsyncReceiver<string>))]
    [TestCase(typeof(IReceiver<string>))]
    public void CreateWrapper(Type receiverType)
    {
            ConfigurableReceiverWrapperFactory sut =
                new ConfigurableReceiverWrapperFactory(false, false, NullPicoLogger.Instance);

            object? receiver = Substitute.For([receiverType], Array.Empty<object>());

            IWrappedReceiver result = sut.CreateWrappedReceiver(receiver, receiverType);

            Assert.That(result, Is.Not.Null);
        }
}