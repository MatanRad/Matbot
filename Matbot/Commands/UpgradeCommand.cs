using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace Matbot.Commands
{
    class UpgradeCommand : Structure.Command
    {
        public UpgradeCommand() : base("upgrade")
        {
            this.RequiredRank = UserRank.Owner;
        }

        public override void Execute(Message message)
        {
            
        }

        public void Execute(Message msg, ulong id, UserRank rank)
        {
            User user = msg.Client.GetChatMemberById(msg.Chat.Id, id);
            if (user == null) return;
            user.ChangeDatabaseRank(rank);

            string name = user.Name;
            if (name == null || name == "") name = user.Username;

            msg.Reply("I upgraded " + id + " (" + name + ")'s rank to: " + rank.ToString());


        }
    }
}
