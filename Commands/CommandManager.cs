using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Commands.Structure.Exceptions;

namespace Matbot.Commands.Structure
{
    class CommandManager
    {
        private static Dictionary<string, Command> Commands = new Dictionary<string, Command>();

        public static void RegisterNewCommand(Command cmd)
        {
            Commands.Add(cmd.Name.ToLower(), cmd);
        }

        public CommandManager()
        {
            
        }

        public static CmdVariation FindCommandVariationByParsedInput(ParsedInput input, Command cmd, out List<object> converted)
        {
            CmdVariation[] vars = cmd.Variations;

            converted = new List<object>();
            List<CmdVariation> conflicts = new List<CmdVariation>();

            string[] param = input.Parameters;

            foreach(CmdVariation v in vars)
            {
                if (v.Attributes.Count != param.Length) continue;
                bool failed = false;
                for (int i=0;i<v.Attributes.Count;i++)
                {
                    CmdAttribute a = v.Attributes[i];
                    try
                    {
                        object t = Convert.ChangeType(param[i], a.AType);
                        converted.Add(t);
                    }
                    catch (InvalidCastException)
                    {
                        failed = true;
                        converted.Clear();
                        break;
                    }
                }

                if(!failed)
                {
                    conflicts.Add(v);
                }
            }

            if (conflicts.Count == 0) throw new CorrectVariationNotFoundException(input);
            if (conflicts.Count > 1) throw new ConflictingVariationsException(input.Name, conflicts.ToArray(), input.RawInput, null);

            return conflicts[0];
        }

        public bool ExecuteUserInput(string input)
        {
            ParsedInput p = new ParsedInput(input);

            return ExecuteUserInput(p);
        }

        public bool ExecuteUserInput(ParsedInput p)
        {
            if (p == null) return false;

            if (p.IsCommand)
            {
                Command c = Commands[p.Name.ToLower()];
                if (c != null)
                {
                    c.ReformatInput(p);

                    List<object> parameters;
                    CmdVariation v = FindCommandVariationByParsedInput(p, c, out parameters);

                    c.ExecuteVariation(v, parameters.ToArray());
                }
                else return false;

            }
            else return false;

            return true;
        }
    }
}
