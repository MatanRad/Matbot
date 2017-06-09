using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Commands.Structure;

namespace MatbotTelegram
{
    class Program
    {
        static void Main(string[] args)
        {
            MatbotDiscord.DiscordClient cl = new MatbotDiscord.DiscordClient("MzEzODI1MzYzMTk5MjYyNzIw.DAJkGQ.kAPnAi6CMbFL1zJDuAVm9mUlf6c", 313825363199262720);
            cl.Start();
            Matbot.ConsoleHider.HideConsole();
            while (true) ;
        }
    }
}
