using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace Matbot.Commands
{
    class FindUserCommand : Matbot.Commands.SingleStringCommand
    {
        public FindUserCommand() : base("finduser")
        {
        }

        public override void Execute(Message message)
        {
            
        }

        public void Execute(Message m, string name)
        {
            User u = m.Client.GetChatMemberByUsername(m.Chat.Id,name);
            m.Reply((u == null ? "Not found." : u.Id.Ids[m.Client.GetClientId()].ToString()));
        }
    }
}
