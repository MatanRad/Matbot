using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Matbot.Commands.Structure.Exceptions;
using Matbot.Client;


namespace Matbot.Commands.Structure
{
    abstract class Command : ICommand
    {
        public string Name
        { get; protected set; }

        public static Type[] InitialTypes = { typeof(Message) };
        public static string executeMethodName = "Execute";
        protected static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

        public UserRank RequiredRank { get; protected set; }
        public bool HasRequiredRank = true;

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
            RequiredRank = UserRank.User;
            Name = name;
            Variations = GetVariations();
        }

        public virtual string GetNoPermissionMessage(Message m)
        {
            return "You have to be at least a " + RequiredRank.ToString() + " to call /" + Name + "!";
        }

        protected CmdVariation FindVariationByMethodInfo(MethodInfo method)
        {
            List<CmdVariation> conflicting = new List<CmdVariation>();
            CmdVariation v = null;

            if (!method.Name.Equals(Command.executeMethodName)) throw new InvalidVariationMethodException(method);

            foreach (CmdVariation i in Variations)
            {
                bool failed = false;

                ParameterInfo[] filtered = FilterOutMethodParameters<ParameterInfo>(method.GetParameters());
                if (i.Attributes.Count != filtered.Length) continue;

                for (int j = 0; j < i.Attributes.Count; j++)
                {
                    if (!(i.Attributes[j].AType.Equals(filtered[j].GetType())
                        && i.Attributes[j].Optional == filtered[j].IsOptional
                        && i.Attributes[j].ParamName.Equals(filtered[j].Name)))
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

            foreach (MethodInfo i in allmethods)
            {
                if (i.Name.Equals(executeMethodName) && i.GetParameters().Length >= Command.InitialTypes.Length)
                {
                    bool increase = true;
                    for(int j=0;j<Command.InitialTypes.Length; j++)
                    {
                        Type t = i.GetParameters()[j].ParameterType;
                        if (!t.Equals(InitialTypes[j]))
                        {
                            increase = false;
                            break;
                        }
                    }
                    if(increase) count++;
                }
            }

            CmdVariation[] vars = new CmdVariation[count];

            for(int i=0;i<count;i++)
            {
                if (allmethods[i].Name.Equals(executeMethodName) && allmethods[i].GetParameters().Length >= Command.InitialTypes.Length)
                {
                    ParameterInfo[] parameters = allmethods[i].GetParameters();

                    bool insert = true;
                    for (int j = 0; j < Command.InitialTypes.Length; j++)
                    {
                        Type t = parameters[j].ParameterType;
                        if (!t.Equals(InitialTypes[j]))
                        {
                            insert = false;
                            break;
                        }
                    }
                    if (insert) vars[i] = new CmdVariation(this.Name, FilterOutMethodParameters<ParameterInfo>(parameters));
                }
            }

            return vars;
        }

        public static T[] FilterOutMethodParameters<T>(T[] info)
        {
            if (info.Length < Command.InitialTypes.Length) return null;

            T[] n = new T[info.Length - Command.InitialTypes.Length];

            for(int i=0;i<n.Length;i++)
            {
                n[i] = info[i + Command.InitialTypes.Length];
            }
            return n;
        }

        public abstract void Execute(Message message);

        public virtual void ReformatInput(ParsedInput input) { }

        public void ExecuteVariation(CmdVariation v, Message msg, object[] parameters)
        {
            MethodInfo m = this.GetType().GetMethod(Command.executeMethodName, Command.bindingFlags, null, v.GetMethodAttributeTypes(), null);
            object[] n = new object[parameters.Length + Command.InitialTypes.Length];
            n[0] = msg;
            parameters.CopyTo(n, Command.InitialTypes.Length);

            if (msg.User.BotRank < RequiredRank && HasRequiredRank) msg.Reply(this.GetNoPermissionMessage(msg));
            else m.Invoke(this, n);
        }

        
    }
}
