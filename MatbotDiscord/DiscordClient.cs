using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Matbot;
using Matbot.Client;

namespace MatbotDiscord
{
    public class DiscordClient : Matbot.Client.Client
    {
        Discord.DiscordClient client;

        public static string DiscordID { get { return "discord"; } }
        public ulong ID { get; private set; }
        private string token;

        public override Matbot.Client.User GetChatMemberById(ChatItemId chatId, ulong id)
        {
            DiscordChatId chat = (DiscordChatId)chatId;
            Discord.Server s = client.GetServer(chat.ServerID);

            foreach (Discord.Channel c in s.AllChannels)
            {
                if(c.Id==chatId.Ids[DiscordID])
                {
                    return UserFromDiscordUser(c.GetUser(id));
                }
            }
            return null;
        }

        public override Chat GetChatById(ChatItemId id)
        {
            return ChatFromDiscordChat(client.GetChannel(id.Ids[DiscordID]));
        }

        private Discord.Channel GetChannelByDiscordChatId(DiscordChatId id)
        {
            if(id.ServerID==0)
            {
                return client.GetChannel(id.Ids[DiscordID]);
            }
            else
            {
                Discord.Server s = client.GetServer(id.ServerID);

                foreach (Discord.Channel c in s.AllChannels)
                {
                    if (c.Id == id.Ids[DiscordID])
                    {
                        return c;
                    }
                }
            }
            
            return null;
        }

        public override Matbot.Client.User GetChatMemberByUsername(ChatItemId chatId, string username, bool exactMatch=true)
        {
            var c = GetChannelByDiscordChatId((DiscordChatId)chatId);
            var u = c.FindUsers(username, exactMatch);

            foreach(Discord.User user in u)
            {
                return UserFromDiscordUser(user);
            }
            return null;
        }

        public override bool SendMessage(ChatItemId id, string message)
        {
            Discord.Channel c = GetChannelByDiscordChatId(id as DiscordChatId);
            if (c == null) return false;

            c.SendMessage(message);

            return true;
        }

        public DiscordClient(Bot bot, DiscordToken token) : base(bot, token)
        {
            ID = token.myid;

            Discord.DiscordConfigBuilder b = new Discord.DiscordConfigBuilder();
            b.AppName = "Matbot";

            client = new Discord.DiscordClient(b);

            client.MessageReceived += C_MessageReceived;
            this.token = token.Token;
        }

        private void C_MessageReceived(object sender, MessageEventArgs e)
        {
            if (e.User.Id == ID) return;

            this.OnMessageReceived(MessageFromEventArgs(e));
        }

        private static DiscordChat ChatFromDiscordChat(Discord.Channel e)
        {
            DiscordChat c;
            if (e.Server == null) c = new DiscordChat(e.Id, ChatType.Channel);
            else c = new DiscordChat(e.Server, e.Id, ChatType.Channel);
            c.Title = e.Topic;
            return c;
        }

        private static DiscordChat ChatFromEventArgs(MessageEventArgs e)
        {
            return ChatFromDiscordChat(e.Channel);
        }

        private Matbot.Client.User UserFromDiscordUser(Discord.User e)
        {
            if (e == null) return null;
            Matbot.Client.User u = this.GetNewUser(e.Id);
            u.Name = e.Name;
            u.Username = e.Nickname;
            return u;
        }
        private Matbot.Client.User UserFromEventArgs(MessageEventArgs e)
        {
            return UserFromDiscordUser(e.User);

        }
        private Matbot.Client.Message MessageFromEventArgs(MessageEventArgs e)
        {
            Matbot.Client.Message m = new Matbot.Client.Message(this, ChatFromEventArgs(e), UserFromEventArgs(e), e.Message.Text);

            return m;
        }

        public override string GetClientId()
        {
            return DiscordID;
        }

        public override bool SendMessage(Chat chat, string message)
        {
            return SendMessage(chat.Id, message);
        }

        public override void Start()
        {
            Running = true;
            client.Connect(token, Discord.TokenType.Bot);
            
        }

        public override void Stop()
        {
            Running = false;
            client.Disconnect();
        }
    }
}
