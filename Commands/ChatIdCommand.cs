using Matbot.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Commands
{
    class ChatIdCommand : Structure.Command
    {
        public ChatIdCommand() : base("chatid")
        {
        }

        public override void Execute(Message message)
        {
            message.Reply("" + message.Chat.Id.Ids[message.Client.GetClientId()]);
        }
    }
}
