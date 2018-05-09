using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Commands.Structure
{
    /// <summary>
    /// Represents a command attribute/parameter.
    /// </summary>
    public class CmdAttribute
    {
        public String ParamName { get; private set; }
        public String Name;

        /// <summary>
        /// Type of method parameter.
        /// </summary>
        public Type AType;

        /// <summary>
        /// Is this attribute/parameter optional.
        /// </summary>
        public bool Optional = false;

        public CmdAttribute(String name, Type atype, bool optional)
        {
            ParamName = name;
            Name = ParamName;
            AType = atype;
            Optional = optional;
        }
    }
}
