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

        private static object ConvertToObj(object value, Type conversionType)
        {
            if (conversionType.IsEnum && (value.GetType().Equals(typeof(string)) || value.GetType().Equals(typeof(String))))
                return Enum.Parse(conversionType, (string)value);

            return Convert.ChangeType(value, conversionType);
        }

        public static CmdVariation FindCommandVariationByParsedInput(ParsedInput input, Command cmd, out List<object> converted)
        {
            CmdVariation[] vars = cmd.Variations;

            converted = new List<object>();
            List<CmdVariation> conflicts = new List<CmdVariation>();

            string[] param = input.Parameters;

            foreach(CmdVariation v in vars)
            {
                if (v == null) continue;
                if (v.Attributes.Count != param.Length) continue;
                bool failed = false;
                for (int i=0;i<v.Attributes.Count;i++)
                {
                    CmdAttribute a = v.Attributes[i];
                    try
                    {
                        object t = ConvertToObj(param[i], a.AType);
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

        public bool ExecuteUserInput(Matbot.Client.Message msg, string input)
        {
            ParsedInput p = new ParsedInput(input);
            return ExecuteUserInput(msg, p);
        }

        public bool ExecuteUserInput(Matbot.Client.Message msg, ParsedInput p)
        {
            if (p == null) return false;

            if(msg.User.BotRank== Client.UserRank.Gali)
            {
                bool c = true;
                if(p.Name!=null)
                {
                    if(p.Name.Equals("register"))
                    {
                        c = false;
                    }
                }
                if(c)
                {
                    msg.Reply(p.RawInput);
                    return false;
                }
            }

            if (p.IsCommand)
            {
                if (!Commands.ContainsKey(p.Name.ToLower())) return false;
                Command c = Commands[p.Name.ToLower()];
                if (c != null)
                {
                    c.ReformatInput(p);

                    List<object> parameters;
                    try
                    {
                        CmdVariation v = FindCommandVariationByParsedInput(p, c, out parameters);

                        c.ExecuteVariation(v, msg, parameters.ToArray());
                    }
                    catch (CorrectVariationNotFoundException ex)
                    {
                        msg.Reply(ex.Message);
                        return false;
                    }
                    
                }
                else return false;

            }
            else return false;

            return true;
        }
    }
}
