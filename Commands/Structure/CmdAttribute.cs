using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Commands.Structure
{
    class CmdAttribute
    {
        public String ParamName { get; private set; }
        public String Name;
        public Type AType;
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
