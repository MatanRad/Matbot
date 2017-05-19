using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Matbot.Commands;

namespace Matbot.Commands.Exceptions
{
    [Serializable]
    class InvalidVariationNameCountException : Exception
    {
        private static string GetFormattedMessage(CmdVariation variation, int nameCount)
        {
            return "Invalid amount of custom names given: " + nameCount + ". " + variation.Attributes.Count + " required!";
        }

        public InvalidVariationNameCountException(CmdVariation variation, int nameCount) : base(GetFormattedMessage(variation, nameCount)) { }

        protected InvalidVariationNameCountException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
