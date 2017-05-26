using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Commands.Structure;

namespace Matbot.Client
{
    public abstract class Client
    {
        CommandManager CmdManger = new CommandManager();

        public abstract void Start();
        public abstract void Stop();

        public virtual void OnMessageReceived(Message message)
        {
            ParsedInput parsed = new ParsedInput(message.Text);

            this.CmdManger.ExecuteUserInput(parsed);
        }

        public Client()
        {
            Matbot.Commands.PlayCommand cmd = new Commands.PlayCommand();

            CommandManager.RegisterNewCommand(cmd);
        }

        public abstract bool SendMessage(Chat chat, string message);

        public virtual bool SendMessage(Chat chat, FormattedMessage message)
        {
            throw new NotImplementedException("Reply for FormattedMessage not yet implemented");
        }
    }
}
