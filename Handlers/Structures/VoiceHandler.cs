using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace Matbot.Handlers.Structure
{
    /// <summary>
    /// A message handler that automatically handles Voice messages.
    /// </summary>
    public abstract class VoiceHandler : IHandler
    {
        public abstract void Handle(Message m);

        public abstract bool IsEnabled();

        public virtual bool ShouldHandle(Message m)
        {
            if (m.Type != MessageType.VoiceMessage) return false;
            return true;
        }

        public abstract void Stop();
    }
}
