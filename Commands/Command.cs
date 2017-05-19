using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Matbot.Commands.Structure.Exceptions;

namespace Matbot.Commands.Structure
{
    abstract class Command : ICommand
    {
        public string Name
        { get; protected set; }
        
        public static string executeMethodName = "Execute";
        protected static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

        private CmdVariation[] variations;

        public CmdVariation[] Variations
        {
            get
            {
                return variations;
            }
            private set
            {
                variations = value;
            }
        }

        public Command(string name)
        {
            Name = name;
            Variations = GetVariations();
        }

        protected CmdVariation FindVariationByMethodInfo(MethodInfo method)
        {
            List<CmdVariation> conflicting = new List<CmdVariation>();
            CmdVariation v = null;

            if (!method.Name.Equals(Command.executeMethodName)) throw new InvalidVariationMethodException(method);

            foreach (CmdVariation i in Variations)
            {
                bool failed = false;
                if (i.Attributes.Count != method.GetParameters().Length) continue;

                for (int j = 0; j < i.Attributes.Count; j++)
                {
                    if (!(i.Attributes[j].AType.Equals(method.GetParameters()[j].GetType())
                        && i.Attributes[j].Optional == method.GetParameters()[j].IsOptional
                        && i.Attributes[j].ParamName.Equals(method.GetParameters()[j].Name)))
                    {
                        failed = true;
                        break;
                    }
                }

                if (!failed)
                {
                    conflicting.Add(i);
                    v = i;
                }
            }

            if (conflicting.Count == 0) throw new InvalidVariationMethodException(method);
            if (conflicting.Count > 1) throw new ConflictingVariationsException(this.Name, conflicting.ToArray(), "", method);

            return v;
        }

        protected void AddCmdVariationDesc(MethodInfo method, string desc, string[] names = null)
        {
            CmdVariation v = FindVariationByMethodInfo(method);

            v.Description = desc;

            if (names != null)
            {
                if (names.Length == 0) return;
                if (names.Length != v.Attributes.Count) throw new InvalidVariationNameCountException(v, names.Length);
                for(int i=0;i<names.Length;i++)
                {
                    v.Attributes[i].Name = names[i];
                }
            }
        }

        private CmdVariation[] GetVariations()
        {
            MethodInfo[] allmethods = this.GetType().GetMethods(Command.bindingFlags);
            int count = 0;

            foreach (MethodInfo i in allmethods) if (i.Name.Equals(executeMethodName)) count++;

            CmdVariation[] vars = new CmdVariation[count];

            for(int i=0;i<count;i++)
            {
                if (allmethods[i].Name.Equals(executeMethodName))
                {
                    ParameterInfo[] parameters = allmethods[i].GetParameters();

                    vars[i] = new CmdVariation(this.Name, parameters);
                }
            }

            return vars;
        }

        public abstract void Execute();

        public virtual void ReformatInput(ParsedInput input) { }

        public void ExecuteVariation(CmdVariation v, object[] parameters)
        {
            MethodInfo m = this.GetType().GetMethod(Command.executeMethodName, Command.bindingFlags, null, v.GetAttributeTypes(), null);

            m.Invoke(this, parameters);
        }

    }
}
