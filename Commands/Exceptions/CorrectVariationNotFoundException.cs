using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Commands;
using System.Runtime.Serialization;

namespace Matbot.Commands.Structure.Exceptions
{
    [Serializable]
    class CorrectVariationNotFoundException : Exception
    {
        private static string GetFormattedMessage(ParsedInput input)
        {
            return "Could not find corresponding variation for input:\n" + input.RawInput;
        }

        public CorrectVariationNotFoundException(ParsedInput input) : base(GetFormattedMessage(input)) { }

        protected CorrectVariationNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
