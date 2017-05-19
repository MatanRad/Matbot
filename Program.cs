using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Commands;
using Matbot.Commands.Structure;

namespace Matbot
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandManager manager = new CommandManager();

            Matbot.Commands.PlayCommand cmd = new Commands.PlayCommand();

            CommandManager.RegisterNewCommand(cmd);

            manager.ExecuteUserInput("/play monstercat frame of mind");
        }
    }
}
