using System;
using picomessenger.wrapper;

namespace picomessenger
{
    public class WrapperFactoryBuilder
    {
        public static Builder Start()
        {
            return new Builder();
        }

        public class Builder
        {
            private IPicoLogger logger;
            private bool disableOnError;
            private bool useWeakReferences;

            public Builder LogErrorsTo(IPicoLogger logger)
            {
                this.logger = logger;
                return this;
            }

            public Builder DisableOnError()
            {
                this.disableOnError = true;
                return this;
            }

            public Builder UseWeakReferences()
            {
                this.useWeakReferences = true;
                return this;
            }

            public IReceiverWrapperFactory Build()
            {
                if (this.logger != null)
                {
                    throw new NotImplementedException();
                }

                if (this.disableOnError)
                {
                    throw new NotImplementedException();
                }

                if (this.useWeakReferences)
                {
                    throw new NotImplementedException();
                }

                return new SimpleReceiverWrapperFactory();
            }
        }
    }
}