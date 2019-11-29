using System;
using Matbot.Client;
using System.Threading;
namespace MatbotCLI
{
    public class CliClient : Client
    {
        Thread cliThread;

        private object lockObject = new object();
        private static int cliId = 0;
        private static int chatId = 100;

        Chat cliChat;
        User cliUser;

        public CliClient(Matbot.Bot bot, ClientToken token) : base(bot, token)
        {
            cliChat = new Chat(GetClientId(), (ulong)chatId, ChatType.Private);
            cliUser = GetNewUser((ulong)cliId);
        }

        public override string GetClientId()
        {
            return "cli";
        }

        public override Chat GetChatById(ChatItemId id)
        {
            if (id[GetClientId()] == (ulong)chatId) return cliChat;
            return null;
        }

        public override bool SendMessage(Chat chat, string message)
        {
            Console.WriteLine("\n" + message + "\n");

            return true;
        }

        private void CliThread()
        {
            while(Running)
            {
                Console.Write(">>  ");
                string s = Console.ReadLine();

                Message m = new Message(this, cliChat, cliUser, s, null, MessageType.TextMessage);

                lock (lockObject)
                    this.OnMessageReceived(m);
            }
        }

        public override void Start()
        {
            if (Running) return;

            Console.WriteLine("CLI Client Initialized");

            Running = true;

            cliThread = new Thread(new ThreadStart(CliThread));
            cliThread.Start();
            
        }

        public override void Stop()
        {
            Console.WriteLine("CLI Session Ended");
            lock (lockObject)
            {
                Running = false;
                cliThread.Abort();
            }
                
            
        }
    }
}
