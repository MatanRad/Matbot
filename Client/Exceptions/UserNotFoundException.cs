using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Client.Exceptions
{
    [Serializable]
    class UserNotFoundException : Exception
    {
        private static string GetFormattedMessage(User input)
        {
            return "User doesn't exist in database: " + input.ToString();
        }

        public UserNotFoundException(User input) : base(GetFormattedMessage(input)) { }

        protected UserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
