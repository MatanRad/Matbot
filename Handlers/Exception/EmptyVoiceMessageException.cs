using System;
using Matbot.Client;

namespace Matbot.Handlers.Exception
{
    class EmptyVoiceMessageException : System.Exception
    {
        private static string GetFormattedMessage(Message input)
        {
            return "Empty Voice Message Received. Id: " + input.Id.ToString();
        }

        public EmptyVoiceMessageException(Message input) : base(GetFormattedMessage(input)) { }
    }
}
