using System;
using System.Threading.Tasks;

namespace picomessenger
{
    public interface IMessengerRegistration
    {
        MessengerErrorPolicy ErrorPolicy { get; set; }
        void SetErrorHandler(Func<Exception, Task<MessengerErrorPolicy>> handler);
    }
}