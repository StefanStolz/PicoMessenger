using picomessenger.wrapper;

namespace picomessenger;

public class WrapperFactoryBuilder
{
    private bool disableOnError;

    private IPicoLogger? logger;
    private bool useWeakReferences;

    public static WrapperFactoryBuilder Start() => new();

    public WrapperFactoryBuilder LogErrorsTo(IPicoLogger logger)
    {
        this.logger = logger;
        return this;
    }

    public WrapperFactoryBuilder DisableOnError()
    {
        this.disableOnError = true;
        return this;
    }

    public WrapperFactoryBuilder UseWeakReferences()
    {
        this.useWeakReferences = true;
        return this;
    }

    public IReceiverWrapperFactory Build()
    {
        if (this.logger != null || this.disableOnError || this.useWeakReferences)
        {
            return new ConfigurableReceiverWrapperFactory(
                this.useWeakReferences,
                this.disableOnError,
                this.logger ?? NullPicoLogger.Instance);
        }

        return new SimpleReceiverWrapperFactory();
    }
}