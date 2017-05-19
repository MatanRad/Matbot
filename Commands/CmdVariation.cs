using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Commands
{
    class CmdVariation
    {
        public string CommandName;
        public List<CmdAttribute> Attributes = new List<CmdAttribute>();
        public string Description = "";
        
        
        public override string ToString()
        {
            string msg = "/" + CommandName + " (";
            foreach (CmdAttribute a in Attributes)
            {
                msg += a.Name + ",";
            }
            msg = msg.Remove(msg.Length - 1, 1) + ")";

            return msg;
        }

        public CmdVariation(string commandName) { CommandName = commandName; }

        public CmdVariation(string commandName,List<CmdAttribute> attr)
        {
            CommandName = commandName;

            Attributes = attr;
        }

        public CmdVariation(string commandName,CmdAttribute[] attr)
        {
            CommandName = commandName;

            InitWithArray(attr);
        }

        public CmdVariation(string commandName,ParameterInfo[] parameters)
        {
            CommandName = commandName;

            if (parameters.Length == 0) return;
            CmdAttribute[] attr = new CmdAttribute[parameters.Length];

            for(int i=0;i<attr.Length;i++)
            {
                attr[i] = new CmdAttribute(parameters[i].Name, parameters[i].GetType(), parameters[i].IsOptional);
            }
        }

        private void InitWithArray(CmdAttribute[] attr)
        {
            Attributes = new List<CmdAttribute>(attr);
        }
    }
}
