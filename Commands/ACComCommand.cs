using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace Matbot.Commands
{
    class ACComCommand : Structure.Command
    {
        public ACComCommand() : base("accom")
        {
        }

        public override void Execute(Message message)
        {
            message.Reply("Current Serial name: " + ACManager.GetACSerialName());
        }
        
        public void Execute(Message message, string name)
        {
            string old = ACManager.GetACSerialName();

            if (ACManager.SetACSerialName(name)) message.Reply("Changed AC Serial name from: \"" + old + "\" to: \"" + name + "\".");
            else message.Reply("New name rejected!");
        }
    }
}
