using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using YoutubeExtractor;
using NAudio.Wave;

namespace Matbot
{
    class YoutubeDownloader
    {
        public static AudioDescriber DownloadAudioWithProgress(YoutubeParser.YoutubeVidDetail detail, TelegramBotClient client, long chatid)
        {
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(detail.URL);
            VideoInfo video = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);

            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            string filename = string.Join("", video.Title.Split(Path.GetInvalidFileNameChars()));
            var videoDownloader = new VideoDownloader(video, Path.Combine("ffmpeg", filename + video.VideoExtension));

            int prev = 1;
            int mul = 25;
            videoDownloader.DownloadProgressChanged += (sender, argss) => 
            {
                int prog = (int)Math.Round(argss.ProgressPercentage);
                if(prog>=prev*mul)
                {
                    client.SendTextMessageAsync(chatid, (mul * prev) + "%");
                    prev++;
                }
            };

            videoDownloader.Execute();
            client.SendTextMessageAsync(chatid, "Converting!");
            Convert(filename);

            File.Delete(Path.Combine("ffmpeg", filename + video.VideoExtension));

            FileStream fstream = new FileStream(Path.Combine("ffmpeg", filename + ".mp3"), System.IO.FileMode.Open);
            MemoryStream mstream = new MemoryStream();
            fstream.CopyTo(mstream);
            fstream.Close();

            Mp3FileReader reader = new Mp3FileReader(Path.Combine("ffmpeg", filename + ".mp3"));
            TimeSpan duration = reader.TotalTime;
            reader.Close();

            File.Delete(Path.Combine("ffmpeg", filename + ".mp3"));
            client.SendTextMessageAsync(chatid, "Success!");

            System.Diagnostics.Debug.WriteLine("\n\nTitle: "+ video.Title + "Duration: " + (int)duration.TotalSeconds);

            return new AudioDescriber((int)duration.TotalSeconds, detail.Channel, filename, mstream);
        }

        public static void Convert(string file)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "ffmpeg\\ffmpeg.exe";
            startInfo.Arguments = " -i \"ffmpeg\\" + file + ".mp4\" -vn -f mp3 -ab 192k \"ffmpeg\\" + file + ".mp3\"";
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }
    }
}
