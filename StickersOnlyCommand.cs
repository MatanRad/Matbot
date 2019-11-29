using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace MatbotTelegram
{
    public enum StickersOnlyParam
    {
        kill,
        Kill
    }

    class StickersOnlyCommand : Matbot.Commands.Structure.Command
    {
        public StickersOnlyCommand() : base("stickersonly")
        {
            this.RequiredRank = UserRank.Admin;
        }

        public void Execute(Message message, StickersOnlyParam param)
        {
            foreach (Matbot.Handlers.Structure.IHandler hnd in message.Client.GetHandlers())
            {
                if(hnd.GetType().Equals(typeof(StickersOnlyHandler)))
                {
                    StickersOnlyHandler h = hnd as StickersOnlyHandler;
                    if (h.GetChatId() == message.Chat.TelegramId()) hnd.Stop();
                }
            }
            message.Reply("Killed Sticker Only Handler for Chat: " + message.Chat.TelegramId());
        }

        public override void Execute(Message message)
        {
            StickersOnlyHandler hnd = new StickersOnlyHandler(message.Chat.TelegramId());
            message.Client.RegisterHandler(hnd);
            message.Reply("Create Sticker Only Handler for Chat: " + message.Chat.TelegramId());
        }
    }
}
