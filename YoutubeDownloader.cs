using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExtractor;

namespace Matbot
{
    class YoutubeDownloader
    {
        public static Client.Audio DownloadAudioWithProgress(YoutubeParser.YoutubeVidDetail detail, Client.Message message)
        {
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(detail.URL);
            VideoInfo video = videoInfos.First(info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);

            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }

            message.Reply("Found!");

            string filename = string.Join("", video.Title.Split(Path.GetInvalidFileNameChars()));
            var videoDownloader = new VideoDownloader(video, Path.GetFullPath(Path.Combine("ffmpeg", filename + video.VideoExtension)));

            int prev = 1;
            int mul = 125;
            videoDownloader.DownloadProgressChanged += (sender, argss) =>
            {
                int prog = (int)Math.Round(argss.ProgressPercentage);
                if (prog >= prev * mul)
                {
                    message.Reply((mul * prev) + "%");
                    prev++;
                }
            };

            videoDownloader.Execute();
            //message.Reply("Converting!");
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
            message.Reply("Success!");

            System.Diagnostics.Debug.WriteLine("\n\nTitle: " + video.Title + "Duration: " + (int)duration.TotalSeconds);

            return new Client.Audio(filename, detail.Channel, (int)duration.TotalSeconds, mstream, "mp3");
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