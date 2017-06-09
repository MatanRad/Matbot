using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Matbot.Client;
using System.Threading.Tasks;
using Telegram.Bot;
using System.Web;

namespace MatbotTelegram
{
    public class TelegramClient : Matbot.Client.Client
    {
        Telegram.Bot.TelegramBotClient TClient;

        private static string ClientId = "telegram";
        private string Token;

        public TelegramClient(string token) :base()
        {
            Token = token;

            TClient = new TelegramBotClient(token);
            TClient.OnUpdate += TClient_OnUpdate;
        }

        public TelegramClient(string token, UserDatabase db) : base(db)
        {
            Token = token;

            TClient = new TelegramBotClient(token);
            TClient.OnUpdate += TClient_OnUpdate;
        }

        public override bool SendMessage(Chat chat, string message)
        {
            TClient.SendTextMessageAsync((long)chat.TelegramId(), message);
            return true;
        }

        private async Task<bool> StartSendingAudioMessageAsync(Chat chat, Matbot.Client.Audio audio)
        {
            try
            {
                using (var httpclient = new System.Net.Http.HttpClient())
                {
                    
                    var uri = "https://api.telegram.org/bot" + Token + "/sendAudio?chat_id=" + chat.TelegramId() + "&title=" + audio.Title + "&duration=" + audio.Duration + "&performer=" + audio.Performer;

                    using (var multipartFormDataContent = new System.Net.Http.MultipartFormDataContent())
                    {
                        var streamContent = new System.Net.Http.StreamContent(audio.AudioStream);
                        streamContent.Headers.Add("Content-Type", "application/octet-stream");
                        streamContent.Headers.Add("Content-Disposition", "form-data; name=\"audio\"; filename=\"" + audio.Title + ".mp3\"");
                        multipartFormDataContent.Add(streamContent, "file", "" + audio.Title + ".mp3");

                        using (var message = await httpclient.PostAsync(HttpUtility.UrlPathEncode(uri), multipartFormDataContent))
                        {
                            var contentString = await message.Content.ReadAsStringAsync();
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public override Chat GetChatById(ChatId id)
        {
            return FromTelegramChat(TClient.GetChatAsync((long)id.Ids[GetClientId()]).Result);
        }
        

        public override bool SendAudioMessage(Chat chat, Matbot.Client.Audio audio)
        {
            StartSendingAudioMessageAsync(chat, audio);
            return true;
        }

        public override void Start()
        {
            TClient.StartReceiving();
        }

        private void TClient_OnUpdate(object sender, Telegram.Bot.Args.UpdateEventArgs e)
        {
            Telegram.Bot.Types.Update u = e.Update;
            if (u.Message.Type != Telegram.Bot.Types.Enums.MessageType.TextMessage) return;
            string text = u.Message.Text;
            Console.WriteLine("Message received: " + text);
            this.OnMessageReceived(FromTelegramMessage(e.Update.Message));
            
        }

        public Message FromTelegramMessage(Telegram.Bot.Types.Message message)
        {
            if (message == null) return null;
            Message m = new Message(this, FromTelegramChat(message.Chat), FromTelegramUser(message.From), message.Text);

            return m;
        }

        public static ChatType FromTelegramChatType(Telegram.Bot.Types.Enums.ChatType type)
        {
            switch (type)
            {
                case Telegram.Bot.Types.Enums.ChatType.Channel:
                    return ChatType.Channel;
                case Telegram.Bot.Types.Enums.ChatType.Group:
                    return ChatType.Group;
                case Telegram.Bot.Types.Enums.ChatType.Private:
                    return ChatType.Private;
                case Telegram.Bot.Types.Enums.ChatType.Supergroup:
                    return ChatType.Supergroup;
                default:
                    return ChatType.Unknown;
            }
        }

        public static Chat FromTelegramChat(Telegram.Bot.Types.Chat chat)
        {
            if (chat == null) return null;
            Chat c = new Chat(ClientId, (ulong)chat.Id, FromTelegramChatType(chat.Type));

            c.Title = chat.Title;
            
            return c;
        }

        public User FromTelegramUser(Telegram.Bot.Types.User user)
        {
            if (user == null) return null;
            User u = GetNewUser((ulong)user.Id);
            string name = "";
            if (user.FirstName != null)
            {
                name += user.FirstName;
                if (user.LastName != null) name += " " + user.LastName;
            }
            u.Name = name;
            u.Username = user.Username;

            return u;
        }

        public override User GetChatMemberById(ChatId chatId, ulong id)
        {
            try
            {
                return FromTelegramUser(TClient.GetChatMemberAsync((long)chatId.Ids[GetClientId()], (int)id).Result.User);
            }
            catch (Exception) { }
            return null;
        }

        public override void Stop()
        {
            TClient.StopReceiving();
        }

        public override string GetClientId()
        {
            return ClientId;
        }
    }
}
