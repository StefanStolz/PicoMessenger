using picomessenger.wrapper;

namespace picomessenger.tests
{
    [TestFixture]
    public class WrapperFactoryBuilderTests
    {
        [Test]
        public void BuildSimpleFactory()
        {
            var sut = WrapperFactoryBuilder.Start();

            var result = sut.Build();

            Assert.That(result, Is.TypeOf<SimpleReceiverWrapperFactory>());
        }

        [Test]
        public void BuildWithLogger()
        {
            var picoLogger = Substitute.For<IPicoLogger>();
            var sut = WrapperFactoryBuilder.Start().LogErrorsTo(picoLogger);

            var result = (sut.Build() as ConfigureableReceiverWrapperFactory) ??
                         throw new AssertionException(
                             $"Result must be of Type {typeof(ConfigureableReceiverWrapperFactory)}");

            Assert.That(result.Logger, Is.SameAs(picoLogger));
            Assert.That(result.UseWeakReferences, Is.False);
            Assert.That(result.DisableOnError, Is.False);
        }


        [Test]
        public void BuildWithWeakReferences()
        {
            var sut = WrapperFactoryBuilder.Start().UseWeakReferences();

            var result = (sut.Build() as ConfigureableReceiverWrapperFactory) ??
                         throw new AssertionException(
                             $"Result must be of Type {typeof(ConfigureableReceiverWrapperFactory)}");

            Assert.That(result.UseWeakReferences, Is.True);
            Assert.That(result.Logger, Is.TypeOf<NullPicoLogger>());
            Assert.That(result.DisableOnError, Is.False);
        }

        [Test]
        public void BuildWithDisableOnError()
        {
            var sut = WrapperFactoryBuilder.Start().DisableOnError();

            var result = (sut.Build() as ConfigureableReceiverWrapperFactory) ??
                         throw new AssertionException(
                             $"Result must be of Type {typeof(ConfigureableReceiverWrapperFactory)}");

            Assert.That(result.DisableOnError, Is.True);
            Assert.That(result.UseWeakReferences, Is.False);
            Assert.That(result.Logger, Is.TypeOf<NullPicoLogger>());
        }
    }
}