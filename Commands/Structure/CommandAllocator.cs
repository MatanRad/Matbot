using Matbot.Commands.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Commands.Structure
{
    class CommandAllocator
    {
        public static void AllocateShared(CommandManager SharedManager)
        {
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
            Matbot.Commands.SayCommand cm13 = new Commands.SayCommand();
            Matbot.Commands.ChatIdCommand cm14 = new Commands.ChatIdCommand();
            Matbot.Commands.VolumeCommand cm15 = new Commands.VolumeCommand();
            var cm16 = new Commands.ServiceCommand();
            var cm17 = new Commands.FiveSecondsCommand();
            var cm18 = new Commands.PyScriptCommand();
            SharedManager.RegisterNewCommand(cmd);
            SharedManager.RegisterNewCommand(cm2);
            SharedManager.RegisterNewCommand(cm3);
            SharedManager.RegisterNewCommand(cm4);
            SharedManager.RegisterNewCommand(cm5);
            SharedManager.RegisterNewCommand(cm6);
            SharedManager.RegisterNewCommand(cm7);
            SharedManager.RegisterNewCommand(cm8);
            SharedManager.RegisterNewCommand(cm9);
            SharedManager.RegisterNewCommand(cm10);
            SharedManager.RegisterNewCommand(cm11);
            SharedManager.RegisterNewCommand(cm12);
            SharedManager.RegisterNewCommand(cm13);
            SharedManager.RegisterNewCommand(cm14);
            SharedManager.RegisterNewCommand(cm15);
            SharedManager.RegisterNewCommand(cm16);
            SharedManager.RegisterNewCommand(cm17);
            SharedManager.RegisterNewCommand(cm18);
        }
    }
}
