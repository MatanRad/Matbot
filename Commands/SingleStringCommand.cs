using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Commands.Structure;

namespace Matbot.Commands
{
    abstract class SingleStringCommand : Structure.Command
    {
        public SingleStringCommand(string name) : base(name) { }

        public override void ReformatInput(ParsedInput input)
        {
            string[] a = new string[1];
            a[0] = String.Join(" ", input.Parameters);
            input.Parameters = a;
        }
    }
}
