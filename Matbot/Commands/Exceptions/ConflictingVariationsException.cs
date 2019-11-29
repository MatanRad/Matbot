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
    /// Exception when a user's input has two conflicting command variations.
    /// </summary>
    [Serializable]
    class ConflictingVariationsException : Exception
    {
        private static string GetFormattedMessage(string cmdName,CmdVariation[] vars, string input, MethodInfo method)
        {
            string methoddesc = "(Not applicable)";
            if (method != null) methoddesc = method.GetFormattedCmdString();
            string msg = "Conflicting variations for cmd \""+cmdName+"\" and variation method:\n"+methoddesc+"\nThe variations:\n";

            foreach(CmdVariation i in vars)
            {
                msg += vars.ToString()+"\n";
            }
            msg += "\nFor the input string:\n"+input;
            return msg;
        }

        public ConflictingVariationsException(string cmdName, CmdVariation[] vars, string input, MethodInfo method) 
            : base(GetFormattedMessage(cmdName, vars, input, method)) { }

        protected ConflictingVariationsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
