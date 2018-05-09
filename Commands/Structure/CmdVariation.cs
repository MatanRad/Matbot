using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Commands.Structure
{
    public class CmdVariation
    {
        public string CommandName;
        public List<CmdAttribute> Attributes = new List<CmdAttribute>();
        public string Description = "";
        
        public Type[] GetAttributeTypes()
        {
            Type[] t = new Type[Attributes.Count];
            for (int i = 0; i < t.Length; i++) t[i] = Attributes[i].AType;

            return t;
        }
        
        public override string ToString()
        {
            string msg = "/" + CommandName;

            foreach (CmdAttribute a in Attributes)
            {
                if (a.Optional) msg += string.Format(" [{0}]", a.Name);
                else msg += string.Format(" <{0}>", a.Name);
            }            

            return msg;
        }

        public string ToStringDetailed()
        {
            string msg = this.ToString();

            if (this.Description != null)
            {
                if (!this.Description.Equals("")) msg += "\nDescription:\n" + this.Description;
            }

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
                //ParameterInfo p = parameters[i];
                attr[i] = new CmdAttribute(parameters[i].Name, parameters[i].ParameterType, parameters[i].IsOptional);
            }

            Attributes = new List<CmdAttribute>(attr);
        }

        private void InitWithArray(CmdAttribute[] attr)
        {
            Attributes = new List<CmdAttribute>(attr);
        }

        internal Type[] GetMethodAttributeTypes()
        {
            Type[] t = new Type[Attributes.Count+Command.InitialTypes.Length];
            t[0] = typeof(Matbot.Client.Message);
            for (int i = Command.InitialTypes.Length; i < t.Length; i++) t[i] = Attributes[i- Command.InitialTypes.Length].AType;

            return t;
        }
    }
}
