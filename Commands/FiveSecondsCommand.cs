using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace Matbot.Commands
{
    class FiveSecondsCommand : Structure.Command
    {
        public FiveSecondsCommand() : base("fiveseconds")
        {

        }

        public override void Execute(Message message)
        {
            Services.FiveSecondCounterService s = new Services.FiveSecondCounterService(message.Client.Bot);
            message.Client.Bot.SrvManager.RegisterNewService(s);
            message.Reply("Registered new fiveseconds!");
        }
    }
}
