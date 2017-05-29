using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace Matbot.Commands
{
    class MyIdCommand : Structure.Command
    {
        public MyIdCommand() : base("myid")
        {
        }

        public override void Execute(Message message)
        {
            message.Reply(""+message.User.Id.Ids[message.Client.GetClientId()]);
        }
    }
}
