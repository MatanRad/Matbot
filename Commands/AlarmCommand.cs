using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;
using Matbot.Commands.Structure;
using System.Diagnostics;
using System.CodeDom.Compiler;
using System.IO;
using System.CodeDom;

namespace Matbot.Commands
{
    class AlarmCommand : Structure.Command
    {
        public enum AlarmOptions
        {
             list, clear
        }

        List<Alarm> alarms = new List<Alarm>();

        class Alarm
        {
            public int hour;
            public int minute;
            public int volume;
            public string link;
            public Process proc;

            public override string ToString()
            {
                return "Alarm: " + hour + ":" + minute + ", vol: " + volume + ", link=" + link;
            }
            
            public Alarm(int h, int m, int v, string l)
            {
                hour = h;
                minute = m;
                volume = v;
                link = l;
            }
        }

        private static string path = "C:\\Users\\matan\\Google Drive\\Projects\\executables\\pyalarmclock.py";

        public AlarmCommand() : base("alarm")
        {
            this.RequiredRank = UserRank.Admin;
        }

        public override void Execute(Message message)
        {
            
        }

        void StartAlarm(Alarm a)
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = "\""+path+"\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardInput = true,
                    CreateNoWindow = false
                }
            };

            proc.Start();
            proc.StandardInput.WriteLine(a.hour);
            proc.StandardInput.WriteLine(a.minute);
            proc.StandardInput.WriteLine(a.volume);
            proc.StandardInput.WriteLine(a.link);
            a.proc = proc;

        }

        private string ToLiteral(string input)
        {
            using (var writer = new StringWriter())
            {
                using (var provider = CodeDomProvider.CreateProvider("CSharp"))
                {
                    provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                    return writer.ToString();
                }
            }
        }

        void StopAlarm(Alarm a)
        {
            try
            {
                a.proc.Kill();
            }
            catch (Exception) { }
        }

        public override void ReformatInput(ParsedInput input)
        {
            if(input.Parameters.Length>3)
            {
                string[] s = new string[4];
                s[0] = input.Parameters[0];
                s[1] = input.Parameters[1];
                s[2] = input.Parameters[2];
                s[3] = "";

                for(int i=3;i<input.Parameters.Length;i++)
                {
                    s[3] += input.Parameters[i] + " ";
                }

                input.Parameters = s;
            }
        }

        public void Execute(Message message, int hour, int minute, int vol, string link)
        {
            Alarm a = new Alarm(hour, minute, vol, link);
            StartAlarm(a);
            alarms.Add(a);
            message.Reply("The following alarm was created: " + a.ToString());
            message.Reply("The link leads to: " + YoutubeParser.ParseVidFromName(a.link).URL);
        }

        public void Execute(Message message, AlarmOptions opt)
        {
            if(opt== AlarmOptions.clear)
            {
                foreach (Alarm a in alarms)
                {
                    StopAlarm(a);
                }
                alarms.Clear();

                message.Reply("Cleared all alarms!");
            }
            else
            {
                
                string s = "List of all alarms:\n";
                for(int i=0;i<alarms.Count;i++)
                {
                    Alarm a = alarms[i];
                    if (a.proc.HasExited)
                    {
                        alarms.RemoveAt(i);
                        i--;
                    }
                    else s += a.ToString()+"\n";
                }

                message.Reply(s);
            }
        }


    }
}
