using picomessenger.wrapper;

namespace picomessenger.tests;

[TestFixture]
public class WrapperFactoryBuilderTests
{
    [Test]
    public void BuildSimpleFactory()
    {
            WrapperFactoryBuilder sut = WrapperFactoryBuilder.Start();

            IReceiverWrapperFactory result = sut.Build();

            Assert.That(result, Is.TypeOf<SimpleReceiverWrapperFactory>());
        }

    [Test]
    public void BuildWithLogger()
    {
            IPicoLogger? picoLogger = Substitute.For<IPicoLogger>();
            WrapperFactoryBuilder sut = WrapperFactoryBuilder.Start().LogErrorsTo(picoLogger);

            ConfigurableReceiverWrapperFactory result = sut.Build() as ConfigurableReceiverWrapperFactory ??
                                                        throw new AssertionException(
                                                            $"Result must be of Type {typeof(ConfigurableReceiverWrapperFactory)}");

            Assert.That(result.Logger, Is.SameAs(picoLogger));
            Assert.That(result.UseWeakReferences, Is.False);
            Assert.That(result.DisableOnError, Is.False);
        }


    [Test]
    public void BuildWithWeakReferences()
    {
            WrapperFactoryBuilder sut = WrapperFactoryBuilder.Start().UseWeakReferences();

            ConfigurableReceiverWrapperFactory result = sut.Build() as ConfigurableReceiverWrapperFactory ??
                                                        throw new AssertionException(
                                                            $"Result must be of Type {typeof(ConfigurableReceiverWrapperFactory)}");

            Assert.That(result.UseWeakReferences, Is.True);
            Assert.That(result.Logger, Is.TypeOf<NullPicoLogger>());
            Assert.That(result.DisableOnError, Is.False);
        }

    [Test]
    public void BuildWithDisableOnError()
    {
            WrapperFactoryBuilder sut = WrapperFactoryBuilder.Start().DisableOnError();

            ConfigurableReceiverWrapperFactory result = sut.Build() as ConfigurableReceiverWrapperFactory ??
                                                        throw new AssertionException(
                                                            $"Result must be of Type {typeof(ConfigurableReceiverWrapperFactory)}");

            Assert.That(result.DisableOnError, Is.True);
            Assert.That(result.UseWeakReferences, Is.False);
            Assert.That(result.Logger, Is.TypeOf<NullPicoLogger>());
        }
}