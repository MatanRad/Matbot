using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Services
{
    /// <summary>
    /// Allocates startup services - meaning there should be only 1 copy at statup.
    /// </summary>
    static class ServiceAllocator
    {
        private static void AllocateStartupService(List<Service> l, Service s, ServiceManager man)
        {
            foreach(Service ser in l)
            {
                if (ser.GetType().Equals(s.GetType())) return;
            }

            man.RegisterNewService(s);
        }

        public static void AllocateStartupServices(Bot bot, List<Service> l, ServiceManager man)
        {
            WonderboyzService s = new WonderboyzService(bot);
            AllocateStartupService(l, s, man);
        }
    }
}
