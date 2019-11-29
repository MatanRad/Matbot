using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;
using System.Diagnostics;

namespace Matbot.Commands
{
    class VolumeCommand : Structure.Command
    {
        protected static string path = "C:\\Users\\matan\\Google Drive\\Projects\\executables\\Remote Manager.exe";

        public VolumeCommand() : base("volume")
        {
        }

        private class Endpoint
        {
            public enum EPLevel
                { Default =0, Comm = 1, Main = 2 }


            public string id;
            public string name;
            public string desc;
            public EPLevel level;
            public int vol;

            public Endpoint() { }

            public void SetDefault()
            {
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = path,
                        Arguments = "-setdef " + name,
                        UseShellExecute = false,
                        RedirectStandardOutput = false,
                        RedirectStandardInput = false,
                        CreateNoWindow = false
                    }

                };

                proc.Start();
            }

            public void SetVolume(int vol)
            {
                this.vol = vol;

                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = path,
                        Arguments = "-setmute " + name + " false",
                        UseShellExecute = false,
                        RedirectStandardOutput = false,
                        RedirectStandardInput = false,
                        CreateNoWindow = false
                    }

                };

                proc.Start();
                proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = path,
                        Arguments = "-setvol "+name+" "+vol,
                        UseShellExecute = false,
                        RedirectStandardOutput = false,
                        RedirectStandardInput = false,
                        CreateNoWindow = false
                    }

                };

                proc.Start();
            }

            public Endpoint(string _id, string _name, string _desc, EPLevel _level, int _vol)
            {
                id = _id;
                name = _name;
                desc = _desc;
                level = _level;
                vol = _vol;
            }

            public Endpoint(string str)
            {
                var s = str.Split(':');
                if (s.Length != 5) throw new ArgumentException();

                id = s[0];
                name = s[1];
                desc = s[2];
                level = (EPLevel)int.Parse(s[3]);
                vol = int.Parse(s[4]);
            }

            public static Endpoint[] fromArray(string[] array)
            {
                List<Endpoint> l = new List<Endpoint>();

                foreach(string i in array)
                {
                    try
                    {
                        l.Add(new Endpoint(i));
                    }
                    catch (OverflowException) { }
                    catch (ArgumentException) { }
                    catch (FormatException) { }
                }

                return l.ToArray();
            }

        }


        private Endpoint[] GetEndpoints()
        {
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = "-reqep",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = false,
                    CreateNoWindow = false
                }

            };

            proc.Start();
            string s = proc.StandardOutput.ReadToEnd();
            
            return Endpoint.fromArray(s.Split(';')); ;
        }


        public override void Execute(Message message)
        {
            string resp = "";
            Endpoint[] eps = GetEndpoints();

            foreach (Endpoint e in eps)
            {
                resp += e.name + " (" + e.desc + ") " + e.vol + ". " + (e.level != Endpoint.EPLevel.Default ? e.level.ToString() + "." : "") + "\n";
            }

            message.Reply(resp);
        }


        public void Execute(Message m, int v)
        {
            if(v<0 || v>100)
            {
                m.Reply("Volume must be between 0 and 100!");
                return;
            }

            foreach (Endpoint e in GetEndpoints())
            {
                if(e.level== Endpoint.EPLevel.Main)
                {
                    e.SetVolume(v);
                    m.Reply("Set volume for main endpoint: \"" + e.name + "\" to " + v);
                }
            }
        }

        public void Execute(Message m, string ep, int v)
        {
            foreach (Endpoint e in GetEndpoints())
            {
                if (e.name.Equals(ep))
                {
                    e.SetDefault();
                    e.SetVolume(v);
                    m.Reply("Set \"" + e.name + "\" as the main endpoint!");
                }
            }
        }
    }
}
