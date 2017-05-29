using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace Matbot.Commands
{
    class SongCommand : SingleStringCommand
    {
        public SongCommand() : base("song")
        {
        }

        public override void Execute(Message message)
        {
            
        }

        public void Execute(Message message, string name)
        {
            Client.Audio audio = YoutubeDownloader.DownloadAudioWithProgress(YoutubeParser.ParseVidFromName(name), message);
            audio.AudioStream.Position = 0;

            message.Client.SendAudioMessage(message.Chat, audio);
        }
    }
}
