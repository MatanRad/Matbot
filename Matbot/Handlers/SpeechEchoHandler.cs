using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace Matbot.Handlers
{
    public class SpeechEchoHandler : Structure.SpeechHandler
    {
        bool enabled = true;

        public override void HandleSpeech(Message m, string text)
        {
            m.Reply(text);
        }

        public override bool IsEnabled()
        {
            return enabled;
        }

        public override void Stop()
        {
            enabled = false;
        }
    }
}
