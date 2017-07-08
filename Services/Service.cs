using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Services
{
    [Serializable]
    public abstract class Service
    {
        [NonSerialized]
        public Bot bot;

        public DateTime LastElapse = DateTime.MinValue;
        public TimeSpan ElapseEvery;
        public DateTime ElapseDate;
        public TimeSpan ElapseTime;

        public bool ElapseEveryActivated = false;
        public bool ElapseDateActivated = false;
        public bool ElapseTimeActivated = false;

        bool ElapseOnce = false;

        public readonly string Desc = "";

        public bool Running = true;

        public int ID;

        public Service(Bot bot)
        {
            this.bot = bot;
        }

        public virtual void Start()
        {
        }

        public virtual void Stop()
        {
            Running = false;
        }

        public virtual bool ShouldElapse()
        {
            if (!Running) return false;
            if (ElapseEveryActivated)
            {
                if (LastElapse + ElapseEvery <= DateTime.Now)
                {
                    if (ElapseOnce) Stop();
                    return true;
                }
            }


            if (ElapseDateActivated)
            {
                if (DateTime.Now >= ElapseDate)
                {
                    if (ElapseOnce) Stop();
                    return true;
                }
            }

            if (ElapseTimeActivated)
            {
                //if (DateTime.MinValue.Equals(LastElapse)) return true;
                if ((DateTime.Now.TimeOfDay >= ElapseTime && LastElapse.Day < DateTime.Now.Day))
                {
                    if (ElapseOnce) Stop();
                    return true;
                }
            }

            return false;
        }

        public abstract void Elapsed();

        public override string ToString()
        {
            int idlen = bot.SrvManager.MaxIDNum.ToString().Length;

            return ID.ToString("D" + idlen) + "    " + GetType().Name + (string.IsNullOrEmpty(Desc) ? "" : " (" + Desc + ")");
        }
    }
}
