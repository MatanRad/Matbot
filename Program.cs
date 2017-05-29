using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatbotTelegram
{
    class Program
    {
        static void Main(string[] args)
        {
            TelegramClient cl = new TelegramClient("220913134:AAHwC-5P5H14sFfx7eDlXYf9qobIx25OFWQ");
            cl.Start();
            Matbot.ConsoleHider.HideConsole();
            while (true) ;
        }
    }
}
