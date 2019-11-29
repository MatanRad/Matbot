using Matbot.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatbotDiscord
{
    class DiscordChat : Matbot.Client.Chat
    {
        public DiscordChat(ulong id, ChatType type) : base(new DiscordChatId(id), type)
        {
        }

        public override bool IsPrivate()
        {
            return true;
        }

        public DiscordChat(Discord.Server Server, ulong id, ChatType type) : base(new DiscordChatId(Server.Id,id), type)
        {
        }        
    }
}
