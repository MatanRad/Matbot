using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Client
{
    public class Audio
    {
        public string Title;
        public string Performer;
        public string ExtensionName;
        public string MimeType;
        public int Duration;
        public Stream AudioStream;

        public string FormattedName
        {
            get
            {
                string s = "";
                if (Title != null) s += Title;
                if (ExtensionName != null) s += "." + ExtensionName;
                return s;
            }
        }


        public Audio() { }
        public Audio(string title, string performer, int dur, Stream stream, string extname = null)
        {
            Title = title;
            Performer = performer;
            Duration = dur;
            AudioStream = stream;

            ExtensionName = extname;
        }

    }
}
