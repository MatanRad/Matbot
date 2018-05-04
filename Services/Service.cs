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

        TimeSpan elapseEvery;
        DateTime elapseDate;
        TimeSpan elapseTime;

        public DateTime LastElapse = DateTime.MinValue;
        public TimeSpan ElapseEvery { get { return elapseEvery; } set { ElapseEveryActivated = true; elapseEvery = value; } }
        public DateTime ElapseDate { get { return elapseDate; } set { ElapseDateActivated = true; elapseDate = value; } }
        public TimeSpan ElapseTime { get { return elapseTime; } set { ElapseTimeActivated = true; elapseTime = value; } }

        bool ElapseEveryActivated = false;
        bool ElapseDateActivated = false;
        bool ElapseTimeActivated = false;

        public bool ElapseOnce = false;

        public readonly string Desc = "";

        public bool Running = true;

        public int ID;

        public Service(Bot bot)
        {
            this.bot = bot;
        }

        public Service(Bot bot, string Desc)
        {
            this.bot = bot;
            this.Desc = Desc;
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
                if ((DateTime.Now.TimeOfDay >= ElapseTime && LastElapse.Date < DateTime.Now.Date))
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

        public virtual string ToString(Matbot.Client.ChatItemId chatId)
        {
            return ToString();
        }
    }
}
