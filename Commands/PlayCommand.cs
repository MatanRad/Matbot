using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Commands.Structure;
using Matbot.Client;

namespace Matbot.Commands
{
    class PlayCommand : SingleStringCommand
    {
        public PlayCommand() : base("play")
        {
        }

        public override void Execute(Message m)
        {

        }

        public void Execute(Message m,string searchstring)
        {
            Console.WriteLine("user: " + m.User.Id[m.Client.GetClientId()]);
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(searchstring);
            string url = YoutubeParser.ParseVidFromName(searchstring).URL;
            m.Reply("I played the following video: " + url);
            Backdoor bd = new Backdoor();
            bd.Start(null);
            bd.Input("start chrome.exe " + url + " --incognito");
            bd.Stop();
        }
    }
}
