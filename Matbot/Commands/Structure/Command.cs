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
    /// <summary>
    /// Represents a bot command.
    /// </summary>
    public abstract class Command : ICommand
    {
        public string Name
        { get; protected set; }

        /// <summary>
        /// Types the Execute methods need to start with.
        /// </summary>
        public static Type[] InitialTypes = { typeof(Message) };
        public static string executeMethodName = "Execute";
        protected static BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public;

        /// <summary>
        /// User rank required to run this command.
        /// </summary>
        public UserRank RequiredRank { get; protected set; }

        /// <summary>
        /// Whether a rank is required to run this command.
        /// </summary>
        public bool HasRequiredRank = true;

        /// <summary>
        /// Priority of parameter type not present in paramTypePriorities.
        /// </summary>
        public static int defaultTypePriority = 0;

        /// <summary>
        /// Mapping from type to integer priority. If not null, used for solving conflicts.
        /// </summary>
        protected Dictionary<Type, int> paramTypePriorities = null;

        private CmdVariation[] variations;

        /// <summary>
        /// All variations of this command.
        /// </summary>
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

        /// <summary>
        /// Returns a string to be replied when a user's rank is not high enough to run this command.
        /// </summary>
        public virtual string GetNoPermissionMessage(Message m)
        {
            return "You have to be at least a " + RequiredRank.ToString() + " to call /" + Name + "!";
        }

        /// <summary>
        /// Find a variation using MethodInfo.
        /// </summary>
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

        /// <summary>
        /// Add description for a command variation.
        /// </summary>
        /// <param name="method">MethodInfo for the variation's method.</param>
        /// <param name="desc">Variation description.</param>
        /// <param name="names">Array of names for each command parameter</param>
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

        /// <summary>
        /// Iterates over all this class' methods, finds and coverts fitting methods to variations.
        /// </summary>
        /// <returns>Array of converted variations.</returns>
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

        /// <summary>
        /// Calculates priority of variation.
        /// </summary>
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
        
        /// <summary>
        /// Tries to resolve conflict by finding a single variation with maximum priority.
        /// </summary>
        /// <param name="conflicts">List of conflicting cmdVariation</param>
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
                    // The maximum priority is held by 2+ variations. Hence conflict still remains.
                    if (argmax != -1) return null;
                    argmax = i;
                }
            }

            return conflicts[argmax];
        }

        /// <summary>
        /// Try to find a single CmdVariation from a user's ParsedInput
        /// </summary>
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

            // Case where only 1 variation succeeds
            converted = convertedPerVar[0];
            return conflicts[0];
        }

        /// <summary>
        /// Removes the first InitialTypes.Length items from 'info'.
        /// </summary>
        /// <typeparam name="T">Type of the given 'info' array.</typeparam>
        /// <param name="info">Array to filter out from.</param>
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

        /// <summary>
        /// Default command variation.
        /// </summary>
        /// <param name="message"></param>
        public abstract void Execute(Message message);

        /// <summary>
        /// When given a user's input, reformat it before finding variations.
        /// </summary>
        /// <param name="input">The input to be reformatted.</param>
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

        /// <summary>
        /// Returns a string detailing each variation.
        /// </summary>
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

        /// <summary>
        /// Returns a string detailing each variation and their respective description.
        /// </summary>
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
