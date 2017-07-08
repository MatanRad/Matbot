using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatbotDiscord
{
    public class DiscordToken : Matbot.Client.ClientToken
    {
        public ulong myid;

        public DiscordToken(string token, ulong myid) : base(typeof(DiscordClient), token)
        {
            this.myid = myid;
        }
    }
}
