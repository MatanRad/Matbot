using Matbot.Commands.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Commands
{
    class CommandAllocator
    {
        private static bool allocated = false;
        public static bool AllocatedShared { get { return allocated; }  }

        public static void AllocateShared()
        {
            if (allocated) return;
            allocated = true;
            CommandManager.SharedManager = new CommandManager();

            Matbot.Commands.PlayCommand cmd = new Commands.PlayCommand();
            Matbot.Commands.RegisterCommand cm2 = new Commands.RegisterCommand();
            Matbot.Commands.MyRankCommand cm3 = new Commands.MyRankCommand();
            Matbot.Commands.MyIdCommand cm4 = new Commands.MyIdCommand();
            Matbot.Commands.UpgradeCommand cm5 = new Commands.UpgradeCommand();
            Matbot.Commands.ACCommand cm6 = new Commands.ACCommand();
            Matbot.Commands.ACComCommand cm7 = new Commands.ACComCommand();
            Matbot.Commands.SongCommand cm8 = new Commands.SongCommand();
            Matbot.Commands.BotConsoleCommand cm9 = new Commands.BotConsoleCommand();
            Matbot.Commands.AlarmCommand cm10 = new Commands.AlarmCommand();
            Matbot.Commands.AddCommand cm11 = new Commands.AddCommand();
            Matbot.Commands.FindUserCommand cm12 = new Commands.FindUserCommand();
            CommandManager.SharedManager.RegisterNewCommand(cmd);
            CommandManager.SharedManager.RegisterNewCommand(cm2);
            CommandManager.SharedManager.RegisterNewCommand(cm3);
            CommandManager.SharedManager.RegisterNewCommand(cm4);
            CommandManager.SharedManager.RegisterNewCommand(cm5);
            CommandManager.SharedManager.RegisterNewCommand(cm6);
            CommandManager.SharedManager.RegisterNewCommand(cm7);
            CommandManager.SharedManager.RegisterNewCommand(cm8);
            CommandManager.SharedManager.RegisterNewCommand(cm9);
            CommandManager.SharedManager.RegisterNewCommand(cm10);
            CommandManager.SharedManager.RegisterNewCommand(cm11);
            CommandManager.SharedManager.RegisterNewCommand(cm12);
        }
    }
}
