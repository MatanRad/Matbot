using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Commands.Structure
{
    class ParsedInput
    {
        public bool IsCommand {
            get
            {
                if (RawInput.Length == 0) return false;
                return RawInput[0] == '/';
            }
        }
        public string Name;
        public string RawInput;
        public string RawParameters;
        public string[] Parameters;

        public ParsedInput(string input)
        {
            RawInput = input;
            string[] split = input.Split(' ');
            Parameters = new string[split.Length-1];

            RawParameters = "";
            for (int i=0;i<Parameters.Length;i++)
            {
                Parameters[i] = split[i + 1];
                RawParameters += Parameters[i];
                if (i != Parameters.Length - 1) RawParameters += " ";
            }

            if (IsCommand) Name = split[0].Substring(1);
        }
    }
}
