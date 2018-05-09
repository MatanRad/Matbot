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
    public abstract class Command : ICommand
    {
        public string Name
        { get; protected set; }

        public static Type[] InitialTypes = { typeof(Message) };
        public static string executeMethodName = "Execute";
        protected static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

        public UserRank RequiredRank { get; protected set; }
        public bool HasRequiredRank = true;

        public static int defaultTypePriority = 0;

        protected Dictionary<Type, int> paramTypePriorities = null;

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
            Variations = InitVariations();
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

        private CmdVariation[] InitVariations()
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

            int ind = 0;

            for(int i=0;i< allmethods.Length; i++)
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
                    if (insert)
                    {
                        vars[ind] = new CmdVariation(this.Name, FilterOutMethodParameters<ParameterInfo>(parameters));
                        ind++;
                    }

                }
            }

            return vars;
        }

        public int CalculateVarPriority(CmdVariation v)
        {
            var keys = this.paramTypePriorities.Keys;

            int priority = 0;

            foreach(CmdAttribute a in v.Attributes)
            {
                if (!keys.Contains(a.AType)) priority += defaultTypePriority;
                else
                {
                    priority += paramTypePriorities[a.AType];
                }
            }

            return priority;
        }
        

        public CmdVariation ResolveConflictViaPriority(List<CmdVariation> conflicts)
        {
            int[] priorities = new int[conflicts.Count];

            for(int i = 0; i < conflicts.Count; i++)
            {
                priorities[i] = CalculateVarPriority(conflicts[i]);
            }

            int max = priorities.Max();
            int argmax = -1;

            for (int i = 0; i < priorities.Length; i++)
            {
                if (priorities[i] == max)
                {
                    if (argmax != -1) return null; // The maximum priority is held by 2+ variations. Hence conflict still remains.
                    argmax = i;
                }
            }

            return conflicts[argmax];
        }

        public  CmdVariation FindCommandVariationByParsedInput(ParsedInput input, out List<object> converted)
        {
            CmdVariation[] vars = Variations;
            var convertedPerVar = new List<List<object>>();
            converted = new List<object>();
            List<CmdVariation> conflicts = new List<CmdVariation>();

            string[] param = input.Parameters;

            foreach (CmdVariation v in vars)
            {
                if (v == null) continue;
                if (v.Attributes.Count != param.Length) continue;
                bool failed = false;
                var convertedVar = new List<object>();
                convertedPerVar.Add(convertedVar);

                for (int i = 0; i < v.Attributes.Count; i++)
                {
                    CmdAttribute a = v.Attributes[i];
                    try
                    {
                        object t = ClassConverter.ConvertToObj(param[i], a.AType);
                        convertedVar.Add(t);
                    }
                    catch (Exception e)
                    {
                        if (e is InvalidCastException || e is FormatException)
                        {
                            System.Diagnostics.Debug.WriteLine(e.GetType().Name);
                            failed = true;
                            convertedPerVar.RemoveAt(convertedPerVar.Count - 1);
                            break;
                        }
                        else throw e;
                    }

                }

                if (!failed)
                {
                    conflicts.Add(v);
                }
            }

            if (conflicts.Count == 0) throw new CorrectVariationNotFoundException(input);
            if (conflicts.Count > 1)
            {
                bool throwError = true;
                if (this.paramTypePriorities != null)
                {
                    CmdVariation resolve = ResolveConflictViaPriority(conflicts);
                    if (resolve != null)
                    {
                        throwError = false;
                        converted = convertedPerVar[conflicts.IndexOf(resolve)];
                        return resolve;
                    }
                }

                if (throwError) throw new ConflictingVariationsException(input.Name, conflicts.ToArray(), input.RawInput, null);
            }

            //case where only 1 variation succeeds
            converted = convertedPerVar[0];
            return conflicts[0];
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

        public override string ToString()
        {
            string s = "";

            for (int i=0; i<Variations.Length; i++)
            {
                s += "[" + (i + 1) + "] " + Variations[i].ToString();
                if (i != Variations.Length - 1) s += "\n";
            }
            return s;
        }

        public string ToStringDetailed()
        {
            string msg = this.ToString();

            bool printedHeader = false;
            for (int i = 0; i < Variations.Length; i++)
            {
                CmdVariation v = Variations[i];

                if (v.Description != null)
                {
                    if (!v.Description.Equals(""))
                    {
                        if(!printedHeader)
                        {
                            printedHeader = true;
                            msg += "Descriptions:";
                        }

                        msg += "\n[" + i + "] " + v.Description; 
                    }
                }
            }
           
            return msg;
        }
    }
}
