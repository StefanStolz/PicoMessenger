using System;
using System.Threading.Tasks;

namespace tinymessenger
{
    public interface IMessengerRegistration
    {
        MesengerErrorPolicy ErrorPolicy { get; set; }
        void SetErrorHandler(Func<Exception, Task<MesengerErrorPolicy>> handler);
    }
}