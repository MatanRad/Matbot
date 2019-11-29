using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace MatbotTelegram
{
    class StickersOnlyHandler : Matbot.Handlers.Structure.IHandler
    {
        ulong id;
        bool running = true;

        public StickersOnlyHandler(ulong chatid)
        {
            id = chatid;
        }

        public ulong GetChatId()
        {
            return id;
        }

        public void Handle(Message m)
        {
            m.Client.DeleteMessage(m); // only executed if m is a non-sticker message
        }

        public bool IsEnabled()
        {
            return running;
        }

        public bool ShouldHandle(Message m)
        {
            return m.Type != MessageType.StickerMessage && m.Type != MessageType.PhotoMessage && m.Client != null && m.Chat.TelegramId() == id;
        }

        public void Stop()
        {
            running = false;
        }
    }
}
