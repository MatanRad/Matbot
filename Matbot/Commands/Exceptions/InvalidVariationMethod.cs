using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Commands.Structure.Exceptions
{
    /// <summary>
    /// Exception when searching a non-existing CommandVariation using MethodInfo. 
    /// </summary>
    [Serializable]
    class InvalidVariationMethodException : Exception
    {
        private static string GetFormattedMessage(MethodInfo method)
        {
            if (method.Name.Equals(Command.executeMethodName))
            {
                return "Cannot find method " + Command.executeMethodName + " in class: " 
                    + method.DeclaringType.Name + " with parameters: " + method.GetParameters().ToString();

            }
            else return "Invalid method given! Method needs to be named: \"" + Command.executeMethodName + "\" but method given is:\n"
                    + Command.executeMethodName + " in class: " + method.DeclaringType.Name 
                    + " with parameters: " + method.GetParameters().ToString();

        }

        public InvalidVariationMethodException(MethodInfo method) : base(GetFormattedMessage(method)) { }

        protected InvalidVariationMethodException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
