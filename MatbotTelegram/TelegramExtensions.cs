using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace MatbotTelegram
{
    static class TelegramExtensions
    {
        public static ulong TelegramId(this Chat chat)
        {
            return chat.Id.Ids["telegram"];
        }

        public static ulong TelegramId(this Message m)
        {
            return m.Id.Ids["telegram"];
        }
    }
}
