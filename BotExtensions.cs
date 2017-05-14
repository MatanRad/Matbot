using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Matbot
{
    class BotExtensions
    {
        public static async void SendAudioFile(string apiid, long chatid, AudioDescriber desc)
        {
            try
            {
                using (var httpclient = new HttpClient())
                {
                    var uri = "https://api.telegram.org/bot" + apiid + "/sendAudio?chat_id=" + chatid + "&title=" + desc.Title + "&duration=" + desc.Duration + "&performer=" + desc.Performer;

                    using (var multipartFormDataContent = new MultipartFormDataContent())
                    {
                        var streamContent = new StreamContent(desc.Stream);
                        streamContent.Headers.Add("Content-Type", "application/octet-stream");
                        streamContent.Headers.Add("Content-Disposition", "form-data; name=\"audio\"; filename=\"" + desc.Title + ".mp3\"");
                        multipartFormDataContent.Add(streamContent, "file", "" + desc.Title + ".mp3");

                        using (var message = await httpclient.PostAsync(HttpUtility.UrlPathEncode(uri), multipartFormDataContent))
                        {
                            var contentString = await message.Content.ReadAsStringAsync();
                        }
                    }
                }
            }
            catch
            {
                return;
            }
            return;
        }
    }
}
