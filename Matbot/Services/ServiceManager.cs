using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// A data structure used for storing and maintaining services.
/// </summary>
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

        /// <summary>
        /// The file where the data structure is saved.
        /// </summary>
        string FilePath;

        public ServiceManager(Bot bot)
        {
            StartTimer();
            MaxIDNum = 50000;

            ServiceAllocator.AllocateStartupServices(bot,RunningServices, this);
        }

        /// <summary>
        /// Starts (or restarts) the timer to the next iteration.
        /// </summary>
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

        /// <summary>
        /// Initialize from saved file (or create if doesn't exist)
        /// </summary>
        /// <param name="bot">The ServiceManager's bot.</param>
        /// <param name="path">File path for saving services.</param>
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

        /// <summary>
        /// Returns the lowest time interval in milliseconds that the next iteration should be.
        /// </summary>
        public long FindLowestInterval()
        {
            if (RunningServices.Count == 0) return (long)Math.Floor(DefaultInterval.TotalMilliseconds);
            long min = RunningServices.Min(new Func<Service, long>(GetServiceInterval));
            if (min <= MinInterval) return (long)Math.Floor(DefaultInterval.TotalMilliseconds);
            else return min;
        }

        /// <summary>
        /// If the service runs on intervals, returns the interval in milliseconds. 0 otherwise.
        /// </summary>
        /// <param name="ser"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Generates an ID for a new service.
        /// </summary>
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

        /// <summary>
        /// Register a service and assign an ID to it.
        /// </summary>
        public int RegisterNewService(Service s)
        {
            s.ID = GetNewID();
            RunningServices.Add(s);
            
            timer.Interval = FindLowestInterval();
            return s.ID;
        }

        /// <summary>
        /// Iterates over services per time period and manages them. 
        /// </summary>
        void ManageServices(object source, ElapsedEventArgs e)
        {
            try
            {
                List<Service> toDelete = new List<Service>();
                foreach (Service s in RunningServices)
                {
                    if (s.ShouldElapse())
                    {
                        s.Elapsed();
                        s.LastElapse = DateTime.Now;
                    }

                    if (!s.Running)
                    {
                        toDelete.Add(s);
                        continue;
                    }
                }

                foreach (Service s in toDelete) RunningServices.Remove(s);
            

                SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error occurred while managing exceptions: " + ex.ToString());
            }

            StartTimer();
        }

        /// <summary>
        /// Saves data structure to file.
        /// </summary>
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
