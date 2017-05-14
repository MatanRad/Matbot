using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot
{
    class AudioDescriber
    {
        public int Duration;
        public string Performer;
        public string Title;
        public Stream Stream;

        public AudioDescriber(int duration, string performer, string title, Stream stream)
        {
            Duration = duration;
            Performer = performer;
            Title = title;
            Stream = stream;
        }
    }

    
}
