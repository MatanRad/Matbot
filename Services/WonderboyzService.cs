using Matbot.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Services
{
    [Serializable]
    class WonderboyzService : RegisterableService
    {
        public WonderboyzService(Bot bot) : base(bot)
        {
            ElapseTime = new TimeSpan(0, 01, 0);
        }

        public override void Elapsed()
        {
            foreach (ChatId i in Registered) bot.BroadcastMessage(i, (DateTime.Now.Subtract(new DateTime(2017, 4, 7)).TotalDays % 2 == 0 ? "Matan" : "Pikh") + " is gay today!" );
        }
    }
}
