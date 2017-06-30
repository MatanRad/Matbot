using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Commands.Structure;

namespace Matbot.Commands
{
    public abstract class SingleStringCommand : Structure.Command
    {
        public SingleStringCommand(string name) : base(name) { }

        public override void ReformatInput(ParsedInput input)
        {
            if (input.Parameters.Length == 0) return;
            string[] a = new string[1];
            a[0] = String.Join(" ", input.Parameters);
            input.Parameters = a;
        }
    }
}
