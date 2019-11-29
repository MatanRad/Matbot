using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace MatbotShared
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> keys = null;

            using (StreamReader r = new StreamReader("keys.json"))
            {
                var json = r.ReadToEnd();
                keys = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }

            // TODO: Add Discord token option
            Matbot.Bot bot = new Matbot.Bot();
            if (!keys["telegram"].Equals("null"))
            {
                var telegramToken = new MatbotTelegram.TelegramToken(keys["telegram"]);
                bot.AddClient(telegramToken);
            }

            var cliToken = new MatbotCLI.CliToken();
            bot.AddClient(cliToken);

            Matbot.Handlers.SpeechEchoHandler h = new Matbot.Handlers.SpeechEchoHandler();
            bot.Clients["telegram"].RegisterHandler(h);

            Matbot.ConsoleHider.HideConsole();

            // TODO: This is ugly. Fix this.
            while (true) System.Threading.Thread.Sleep(2000);
        }
    }
}
