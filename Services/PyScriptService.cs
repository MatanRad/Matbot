using Matbot.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Services
{
    [Serializable]
    class PyScriptService : RegisterableService
    {
        string file;

        bool IsScriptRunning = false;

        public static string PyScriptPath = "Scripts/";

        bool StopAfterOutput = false;

        public PyScriptService(Bot bot, int seconds, string file, Commands.PyScriptCommand.PyScriptOnce Once) : base(bot,file)
        {
            this.file = file;
            ElapseEvery = TimeSpan.FromSeconds(seconds);

            if (!File.Exists(Path.Combine(PyScriptPath, file))) Stop();

            if (Once == Commands.PyScriptCommand.PyScriptOnce.once) this.ElapseOnce = true;
            else if (Once == Commands.PyScriptCommand.PyScriptOnce.onceuntiloutput) StopAfterOutput = true;
        }

        public override void Elapsed()
        {
            if(!IsScriptRunning) Task.Factory.StartNew(() => RunScript());
        }

        void RunScript()
        {
            IsScriptRunning = true;
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = "\"" + Path.Combine(PyScriptPath, file) + "\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = false,
                    CreateNoWindow = false
                }
            };

            proc.Start();
            string output = "";

            while (!proc.HasExited)
            {
                output += proc.StandardOutput.ReadToEnd();
            }

            if (StopAfterOutput && !string.IsNullOrWhiteSpace(output)) Stop();

            if (!string.IsNullOrWhiteSpace(output))
                foreach (ChatItemId id in Registered) bot.BroadcastMessage(id, "Python Script \"" + file + "\" finished execution with output:\n" + output);

            IsScriptRunning = false;
        }
    }
}
