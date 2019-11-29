using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Commands.Structure.Exceptions;

namespace Matbot.Commands.Structure
{
    /// <summary>
    /// Data Structure for storing and communicating with commands.
    /// </summary>
    public class CommandManager
    {
        /// <summary>
        /// A mapping from command name to Command.
        /// </summary>
        private Dictionary<string, Command> Commands = new Dictionary<string, Command>();

        /// <summary>
        /// Returns array of CommandDescriptors for every command stored.
        /// </summary>
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

        /// <summary>
        /// Given a message, find corresponding command and run it.
        /// </summary>
        /// <param name="msg">The message that ran the command.</param>
        /// <param name="input">The input from the user.</param>
        public bool ExecuteUserInput(Matbot.Client.Message msg, string input)
        {
            ParsedInput p = new ParsedInput(input);
            return ExecuteUserInput(msg, p);
        }

        /// <summary>
        /// Given a message, find corresponding command and run it.
        /// </summary>
        /// <param name="msg">The message that ran the command.</param>
        /// <param name="input">The input from the user.</param>
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
