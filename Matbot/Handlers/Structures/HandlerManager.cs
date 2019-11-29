using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;
namespace Matbot.Handlers.Structure
{
    /// <summary>
    /// Data Structure for managing message handlers.
    /// </summary>
    public class HandlerManager
    {
        public List<IHandler> handlers = new List<IHandler>();

        /// <summary>
        /// Registers a new message handler.
        /// </summary>
        public void Register(IHandler handler)
        {
            if (handler.IsEnabled()) handlers.Add(handler);
        }

        /// <summary>
        /// Iterates over all handlers and runs the ones that are compatible with this message.
        /// </summary>
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
