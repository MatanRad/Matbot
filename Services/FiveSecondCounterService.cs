using Matbot.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Services
{
    [Serializable]
    class FiveSecondCounterService : RegisterableService
    {
        public FiveSecondCounterService(Bot bot) : base(bot)
        {
            ElapseEvery = new TimeSpan(0, 0, 5);
        }

        public override void Elapsed()
        {
            foreach(ChatId c in Registered)
            {
                bot.BroadcastMessage(c, "5 seconds!");
            }
        }
    }
}
