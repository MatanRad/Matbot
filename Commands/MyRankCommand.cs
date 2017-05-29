using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace Matbot.Commands
{
    class MyRankCommand : Structure.Command
    {
        public MyRankCommand() : base("myrank")
        {
        }

        public override void Execute(Message message)
        {
            message.Reply("Your rank is: " + message.User.BotRank.ToString());
        }
    }
}
