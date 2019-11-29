using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatbotTelegram
{
    public class TelegramToken : Matbot.Client.ClientToken
    {
        public TelegramToken(string token) : base(typeof(TelegramClient), token)
        {

        }
    }
}
