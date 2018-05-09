using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Commands.Structure
{
    public class CommandDescriptor
    {
        private Command command;

        public CmdVariation[] Variations
        {
            get
            {
                return command.Variations;
            }
        }

        public string Name
        {
            get
            {
                return command.Name;
            }
        }


        public CommandDescriptor(Command c)
        {
            this.command = c;
        }

        public override string ToString()
        {
            return command.ToString();
        }

        public string ToStringDetailed()
        {
            return command.ToStringDetailed();
        }
    }
}
