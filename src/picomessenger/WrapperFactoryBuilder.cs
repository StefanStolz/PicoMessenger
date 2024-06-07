using picomessenger.wrapper;

namespace picomessenger
{
    public class WrapperFactoryBuilder
    {
        public static WrapperFactoryBuilder Start()
        {
            return new WrapperFactoryBuilder();
        }

        private IPicoLogger? logger;
        private bool disableOnError;
        private bool useWeakReferences;

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
            if (this.logger != null || this.disableOnError || this.useWeakReferences) {
                return new ConfigurableReceiverWrapperFactory(
                    this.useWeakReferences,
                    this.disableOnError,
                    this.logger ?? NullPicoLogger.Instance);
            }

            return new SimpleReceiverWrapperFactory();
        }
    }
}
