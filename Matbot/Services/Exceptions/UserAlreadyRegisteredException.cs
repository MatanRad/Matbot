using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Services.Exceptions
{
    [Serializable]
    class UserAlreadyRegisteredException : Exception
    {
        private static string GetFormattedMessage(Matbot.Services.Service service)
        {
            return "User already registered in service: " + service.ToString();
        }

        public UserAlreadyRegisteredException(Matbot.Services.Service service) : base(GetFormattedMessage(service)) { }

        protected UserAlreadyRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
