using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;
using Matbot.Services;

namespace Matbot.Commands
{
    class PyScriptCommand : Structure.Command
    {
        public enum PyScriptOnce
        {
            repeat,
            once,
            onceuntiloutput,
        }


        public PyScriptCommand() : base("pyscript")
        {

        }

        public override void Execute(Message message)
        {
            message.Reply("Usage: /pyscript [seconds interval] [file] (\"once\"  /  \"onceuntiloutput\")");
        }

        public void Execute(Message m, int seconds, string file, PyScriptOnce once)
        {
            StartNew(m, seconds, file, once);
        }

        public void Execute(Message m, int seconds, string file)
        {
            StartNew(m, seconds, file, PyScriptOnce.repeat);
        }

        void StartNew(Message m, int seconds, string file, PyScriptOnce once)
        {
            PyScriptService ser = new PyScriptService(m.Client.Bot, seconds, file, once);
            int id = m.Client.Bot.SrvManager.RegisterNewService(ser);
            m.Reply("PyScript Service started with ID: " + id + ". This chat was registered to it.");
            ser.Register(m.Chat);
        }
    }
}
