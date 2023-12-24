using System;
using System.Threading.Tasks;

namespace picomessenger
{
    public interface IMessengerRegistration
    {
        MesengerErrorPolicy ErrorPolicy { get; set; }
        void SetErrorHandler(Func<Exception, Task<MesengerErrorPolicy>> handler);
    }
}