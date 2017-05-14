using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot
{
    class LyricsFinder
    {

        public static string FindLyrics(string name)
        {
            try
            {
                string html = HttpGetter.GetHTMLContent(GetAZLink(name));
                string start = "<!-- Usage of azlyrics.com content by any third-party lyrics provider is prohibited by our licensing agreement. Sorry about that. -->";
                string end = "</div>";
                html = html.Substring(html.IndexOf(start)+start.Length);
                html = html.Substring(0, html.IndexOf(end));
                html = System.Net.WebUtility.HtmlDecode(html);
                html = html.Replace("<br>", "");
                return html;
            }
            catch
            {
            }
            return "Couldn't find lyrics, sorry!";
        }

        public static string GetAZLink(string name)
        {
            try
            {
                string html = HttpGetter.GetHTMLContent("http://search.azlyrics.com/search.php?q=" + System.Net.WebUtility.UrlEncode(name));
                html = html.Substring(html.IndexOf("http://www.azlyrics.com/lyrics/"));
                string link = html.Substring(0, html.IndexOf("\"") );
                return link;
            }
            catch
            {
            }
            return null;
        }
    }
}
