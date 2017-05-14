using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot
{
    public class Command
    {
        public string Name {  get; private set; }
        public List<string> parameters = new List<string>();

        public string Raw()
        {
            string str = Name;
            str += " " + RawParams();
            return str;
        }

        public string RawParams()
        {
            string str = "";
            foreach (string s in parameters)
            {
                if (s != parameters[0]) str += " ";
                str +=  s;
            }
            return str;
        }

        public Command(string cmd)
        {
            int spaceIndex = cmd.IndexOf(' ');
            if (spaceIndex == -1) Name = cmd;
            else
            {
                Name = cmd.Substring(0, spaceIndex);
                cmd = cmd.Substring(spaceIndex + 1);
                parameters = new List<string>(Global.CommandLineToArgs(cmd));
            }
        }
        

    }
}
