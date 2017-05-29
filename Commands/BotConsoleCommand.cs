using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace Matbot.Commands
{
    class BotConsoleCommand : Structure.Command
    {
       


        public BotConsoleCommand() : base("botconsole")
        {
        }

        public override void Execute(Message message)
        {
            
        }
        
        public void Execute(Message message, string s)
        {
            if (s.ToLower().Equals("show") || s.ToLower().Equals("hide"))
            {
                if (s.ToLower().Equals("show")) Matbot.ConsoleHider.ShowConsole();
                else Matbot.ConsoleHider.HideConsole();
            }
            else message.Reply("Usage: /botconsole [show/hide]");
        }
    }
}
