using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Matbot.Services
{
    [Serializable]
    public class ServiceManager
    {
        List<Service> RunningServices = new List<Service>();

        /// <summary>
        /// The default elapsing interval to wait between service activations.
        /// </summary>
        public TimeSpan DefaultInterval = new TimeSpan(0, 0, 5);

        /// <summary>
        /// The minimal interval (in milliseconds) in which the timer can elapse
        /// </summary>
        public long MinInterval = 250;

        public int MaxIDNum { get; private set; }

        [NonSerialized]
        Timer timer;

        string FilePath;

        public ServiceManager(Bot bot)
        {
            StartTimer();
            MaxIDNum = 50000;

            ServiceAllocator.AllocateStartupServices(bot,RunningServices, this);
        }

        private void StartTimer()
        {
            if (timer == null)
            {
                timer = new Timer();
                timer.Elapsed += ManageServices;
                timer.AutoReset = false;
                timer.Interval = FindLowestInterval();
            }

            timer.Start();

        }

        public ServiceManager(Bot bot, string path)
        {
            MaxIDNum = 50000;
            FilePath = path;
            StartTimer();
            try
            {
                using (var ms = new FileStream(FilePath, FileMode.Open))
                {
                    var set = new BinaryFormatter();
                    RunningServices = set.Deserialize(ms) as List<Service>;
                }
            }
            catch (FileNotFoundException) { }
            catch (SerializationException) { }
            catch (ArgumentException) { }

            foreach (Service s in RunningServices)
            {
                s.bot = bot;
                if (s.ID == 0) s.ID = GetNewID();
            }

            ServiceAllocator.AllocateStartupServices(bot,RunningServices, this);
            
        }

        public Service FindServiceByID(int id)
        {
            return RunningServices.Find(x => x.ID == id);
        }

        public long FindLowestInterval()
        {
            if (RunningServices.Count == 0) return (long)Math.Floor(DefaultInterval.TotalMilliseconds);
            long min = RunningServices.Min(new Func<Service, long>(GetServiceInterval));
            if (min <= MinInterval) return (long)Math.Floor(DefaultInterval.TotalMilliseconds);
            else return min;
        }

        long GetServiceInterval(Service ser)
        {
            if (ser.ElapseEvery != null) return (long)Math.Floor(ser.ElapseEvery.TotalMilliseconds);
            return 0;
        }
        int GetServiceID(Service ser)
        {
            if (ser == null) return 0;
            return ser.ID;
        }

        int GetNewID()
        {
            bool found = true;
            int nid = 0;
            Random r = new Random();
            while (found || nid==0)
            {
                nid = r.Next(1, this.MaxIDNum);

                found = FindServiceByID(nid) != null;
            }

            return nid;
        }

        public int RegisterNewService(Service s)
        {
            s.ID = GetNewID();
            RunningServices.Add(s);
            
            timer.Interval = FindLowestInterval();
            return s.ID;
        }

        void ManageServices(object source, ElapsedEventArgs e)
        {
            try
            {
                foreach (Service s in RunningServices)
                {
                    if (s.ShouldElapse())
                    {
                        s.Elapsed();
                        s.LastElapse = DateTime.Now;
                    }

                    if (!s.Running)
                    {
                        RunningServices.Remove(s);
                        continue;
                    }
                }
            

                SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while managing exceptions: " + ex.ToString());
            }

            StartTimer();
        }

        private void SaveChanges()
        {
            if (FilePath == null) return;
            try
            {
                using (var ms = new FileStream(FilePath, FileMode.Create))
                {
                    var set = new BinaryFormatter();
                    set.Serialize(ms, RunningServices);
                }
            }
            catch (SerializationException e)
            {
                Console.WriteLine(e.ToString());
            }
            
        }

        public string GetAllServicesString()
        {
            return GetAllServicesString(null);
        }

        public string GetAllServicesString(Matbot.Client.ChatItemId id)
        {
            string s = "";
            foreach (Service ser in RunningServices)
            {
                if (id==null) s += ser.ToString() + "\n";
                else s += ser.ToString(id) + "\n";
            }
            return s;
        }
    }
}
