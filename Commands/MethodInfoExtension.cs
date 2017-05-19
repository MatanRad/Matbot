using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Commands.Structure
{
    public static class MethodInfoExtension
    {
        public static string GetFormattedCmdString(this MethodInfo method)
        {
            string msg = method.DeclaringType.Name + "." + method.Name + "(";
            foreach(ParameterInfo p in method.GetParameters())
            {
                msg += p.GetType().Name + " " + p.Name+",";
            }
            msg = msg.Remove(msg.Length - 1, 1) + ")";

            return msg;
        }
    }
}
