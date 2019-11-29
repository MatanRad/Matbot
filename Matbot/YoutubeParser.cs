using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot
{
    class YoutubeParser
    {
        public struct YoutubeVidDetail
        {
            public string URL;
            public string Channel;

            public YoutubeVidDetail(string url, string channel)
            {
                URL = url;
                Channel = channel;
            }
        }

        public static YoutubeVidDetail ParseVidFromName(string name)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(name);
            string[] dets = HttpGetter.GetHTMLContent("http://kindvideo.xyz/youtube/?b64=" + System.Convert.ToBase64String(plainTextBytes)).Split(';');
            return new YoutubeVidDetail("http://www.youtube.com/watch?v=" + dets[0], dets[1]);
        }
    }
}
