using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;
namespace Matbot.Handlers.Structure
{
    public class HandlerManager
    {
        public List<IHandler> handlers = new List<IHandler>();

        public void Register(IHandler handler)
        {
            if (handler.IsEnabled()) handlers.Add(handler);
        }

        public void MessageReceieved(Message m)
        {
            List<IHandler> toRemove = new List<IHandler>();
            foreach (IHandler h in handlers)
            {
                if (!h.IsEnabled()) toRemove.Add(h);
                else
                {
                    if (h.ShouldHandle(m)) h.Handle(m);
                }

            }

            foreach (IHandler h in toRemove)
            {
                handlers.Remove(h);
            }
        }
    }
}
