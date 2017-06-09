using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatbotDiscord
{
    class DiscordChatId : Matbot.Client.ChatId
    {
        public DiscordChatId(ulong id) : base(DiscordClient.DiscordID,id)
        {

        }

        public DiscordChatId(ulong serverId,ulong id) : base(DiscordClient.DiscordID, id)
        {
            this.ServerID = serverId;
        }

        public ulong ServerID;
    }
}
