using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Commands.Structure
{
    /// <summary>
    /// Represents a variation of a command.
    /// </summary>
    public class CmdVariation
    {
        public string CommandName;
        public List<CmdAttribute> Attributes = new List<CmdAttribute>();
        public string Description = "";
        
        /// <summary>
        /// Returns a type array of the types of each parameter in the variation method.
        /// </summary>
        public Type[] GetAttributeTypes()
        {
            Type[] t = new Type[Attributes.Count];
            for (int i = 0; i < t.Length; i++) t[i] = Attributes[i].AType;

            return t;
        }
        
        /// <summary>
        /// Returns string visualzation of variation.
        /// </summary>
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

        /// <summary>
        /// Returns string visualzation of variation, with description if available.
        /// </summary>
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

        /// <summary>
        /// Returns type array corresponding with a Command method (e.g. with Message and InitialTypes at beggining).
        /// </summary>
        internal Type[] GetMethodAttributeTypes()
        {
            Type[] t = new Type[Attributes.Count+Command.InitialTypes.Length];
            t[0] = typeof(Matbot.Client.Message);
            for (int i = Command.InitialTypes.Length; i < t.Length; i++) t[i] = Attributes[i- Command.InitialTypes.Length].AType;

            return t;
        }
    }
}
