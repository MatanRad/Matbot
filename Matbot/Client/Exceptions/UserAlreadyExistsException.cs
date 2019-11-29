using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Client.Exceptions
{
    /// <summary>
    /// Exception when trying to add an already existing user in UserDatabase.
    /// </summary>
    [Serializable]
    class UserAlreadyExistsException : Exception
    {
        private static string GetFormattedMessage(User input)
        {
            return "User already exists in database: " + input.ToString();
        }

        public UserAlreadyExistsException(User input) : base(GetFormattedMessage(input)) { }

        protected UserAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
