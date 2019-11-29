using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace Matbot.Commands
{
    class ACCommand : Structure.Command
    {
        public class ACAttr
        {
            public int temp = 20;
            public bool power = false;
            public ACPower level = ACPower.high;

            public ACAttr(ACAttr prev, bool p)
            {
                temp = prev.temp;
                level = prev.level;
                power = p;
            }
            public ACAttr(bool p) { power = p; }
            public ACAttr(bool p, int temp, ACPower l)
            {
                power = p;
                this.temp = temp;
                level = l;
            }
        }

        private ACAttr LastUsedAttr = new ACAttr(false);

        public ACCommand() : base("ac")
        {
            RequiredRank = UserRank.Admin;
        }


        public override void Execute(Message message)
        {
            
        }

        public void Execute(Message message, string onoff)
        {
            if(onoff.Equals("on") || onoff.Equals("off"))
            {
                ACAttr att = new ACAttr(LastUsedAttr,onoff.Equals("on"));
                if (ACManager.SendAC(att.temp, att.power, att.level)) message.Reply("Transmitted!");
                else message.Reply("An error occurred while transmitting!");
            }
            else
            {
                message.Reply("Usage: /ac [on/off]");
            }
        }

        public void Execute(Message message, string onoff, int temp, ACPower level)
        {
            if (onoff.Equals("on") || onoff.Equals("off"))
            {
                LastUsedAttr = new ACAttr(onoff.Equals("on"), temp, level);
                if(ACManager.SendAC(LastUsedAttr.temp, LastUsedAttr.power, LastUsedAttr.level)) message.Reply("Transmitted!");
                else message.Reply("An error occurred while transmitting!");
            }
            else
            {
                message.Reply("Usage: /ac [on/off] {temp} {low/med/high/auto/turbo}");
            }
            
        }
    }
}
