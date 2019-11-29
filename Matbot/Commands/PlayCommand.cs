using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Commands.Structure;
using Matbot.Client;
using System.Diagnostics;

namespace Matbot.Commands
{
    class PlayCommand : SingleStringCommand
    {
        Process lastopened = null;
        public PlayCommand() : base("play")
        {
        }

        private void StopLast()
        {
            if (lastopened == null) return;
            try
            {
                lastopened.Kill();
            }
            catch (Exception) { }
        }

        public override void Execute(Message m)
        {
            StopLast();
        }

        public void Execute(Message m,string searchstring)
        {
            StopLast();
            Console.WriteLine("user: " + m.User.Id[m.Client.GetClientId()]);
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(searchstring);
            string url = YoutubeParser.ParseVidFromName(searchstring).URL;
            m.Reply("I played the following video: " + url);

            lastopened = Process.Start("chrome.exe", url + " --incognito --user-data-dir=\"%apppdata%/Matbot/Chromesettings\"");
            /*Backdoor bd = new Backdoor();
            bd.Start(null);
            bd.Input("start chrome.exe " + url + " --incognito");
            bd.Stop();*/

        }
    }
}
