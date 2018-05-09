using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Services
{
    /// <summary>
    /// A service/proccess that runs on time based periods.
    /// </summary>
    [Serializable]
    public abstract class Service
    {
        [NonSerialized]
        public Bot bot;

        TimeSpan elapseEvery;
        DateTime elapseDate;
        TimeSpan elapseTime;

        public DateTime LastElapse = DateTime.MinValue;

        /// <summary>
        /// If not null, runs every given timespan (interval).
        /// </summary>
        public TimeSpan ElapseEvery { get { return elapseEvery; } set { ElapseEveryActivated = true; elapseEvery = value; } }

        /// <summary>
        /// If not null, run (once) at give DateTime.
        /// </summary>
        public DateTime ElapseDate { get { return elapseDate; } set { ElapseDateActivated = true; elapseDate = value; } }

        /// <summary>
        /// If not null, run every day at defined time.
        /// </summary>
        public TimeSpan ElapseTime { get { return elapseTime; } set { ElapseTimeActivated = true; elapseTime = value; } }

        bool ElapseEveryActivated = false;
        bool ElapseDateActivated = false;
        bool ElapseTimeActivated = false;

        /// <summary>
        /// Should this service stop after running.
        /// </summary>
        public bool ElapseOnce = false;

        /// <summary>
        /// Description of the service.
        /// </summary>
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

        /// <summary>
        /// Returns whether the service should run.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Execute service.
        /// </summary>
        public abstract void Elapsed();

        /// <summary>
        /// Returns string describing the service.
        /// </summary>
        public override string ToString()
        {
            int idlen = bot.SrvManager.MaxIDNum.ToString().Length;

            return ID.ToString("D" + idlen) + "    " + GetType().Name + (string.IsNullOrEmpty(Desc) ? "" : " (" + Desc + ")");
        }

        /// <summary>
        /// Returns string describing the service with respect to a Chat.
        /// </summary>
        public virtual string ToString(Matbot.Client.ChatItemId chatId)
        {
            return ToString();
        }
    }
}
