using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Services.Exceptions
{
    [Serializable]
    class UserNotRegisteredException : Exception
    {
        private static string GetFormattedMessage(Matbot.Services.Service service)
        {
            return "User not registered in service: " + service.ToString();
        }

        public UserNotRegisteredException(Matbot.Services.Service service) : base(GetFormattedMessage(service)) { }

        protected UserNotRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
