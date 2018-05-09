using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Commands.Structure.Exceptions;

namespace Matbot.Commands.Structure
{
    public class CommandManager
    {
        //public static CommandManager SharedManager;

        private Dictionary<string, Command> Commands = new Dictionary<string, Command>();

        public CommandDescriptor[] GetCommandDescriptors()
        {
            var keys = Commands.Keys.ToArray();

            if (keys.Length == 0) return null;

            CommandDescriptor[] descs = new CommandDescriptor[keys.Length];

            for(int i=0; i<descs.Length; i++)
            {
                descs[i] = new CommandDescriptor(Commands[keys[i]]);
            }

            return descs;
        }

        public void RegisterNewCommand(Command cmd)
        {
            Commands.Add(cmd.Name.ToLower(), cmd);
        }

        public CommandManager()
        {
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
                    return true;
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
                        CmdVariation v = c.FindCommandVariationByParsedInput(p, out parameters);

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
