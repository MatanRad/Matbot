using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Commands.Structure;
using Matbot;

namespace Matbot.Commands
{
    class PlayCommand : SingleStringCommand
    {
        public PlayCommand() : base("play")
        {
        }

        public override void Execute()
        {

        }

        public void Execute(string searchstring)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(searchstring);
            string url = YoutubeParser.ParseVidFromName(searchstring).URL;
            Backdoor bd = new Backdoor();
            bd.Start(0);
            bd.Input("start chrome.exe " + url + " --incognito");
            bd.Stop();
        }
    }
}
