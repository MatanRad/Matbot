using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace Matbot.Commands
{
    class AddCommand : Structure.Command
    {
        public AddCommand() : base("add")
        {
        }

        public override void Execute(Message message)
        {
            
        }

        public void Execute(Message m,double a, double b)
        {
            m.Reply((a+b).ToString());
        }
    }
}
