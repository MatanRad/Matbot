using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Matbot.Client;
using System.Threading.Tasks;
using Telegram.Bot;
using System.Web;
using System.Net;

namespace MatbotTelegram
{
    public class TelegramClient : Matbot.Client.Client
    {
        Telegram.Bot.TelegramBotClient TClient;

        private static string ClientId = "telegram";
        private string Token;
        private static Telegram.Bot.Types.Enums.MessageType[] SupportedTypes = {
            Telegram.Bot.Types.Enums.MessageType.TextMessage,
            Telegram.Bot.Types.Enums.MessageType.VoiceMessage,
            Telegram.Bot.Types.Enums.MessageType.AudioMessage
        };

        public TelegramClient(Matbot.Bot bot, ClientToken token) :base(bot, token)
        {
            Token = token.Token;

            TClient = new TelegramBotClient(token.Token);
            TClient.OnUpdate += TClient_OnUpdate;

            CustomCmdManger.RegisterNewCommand(new StickersOnlyCommand());
        }


        public override bool SendMessage(Chat chat, string message)
        {
            TClient.SendTextMessageAsync((long)chat.TelegramId(), message);
            return true;
        }

        public override bool DeleteMessage(Message m)
        {
            Chat chat = m.Chat;
            try
            {
                using (var httpclient = new System.Net.Http.HttpClient())
                {

                    var uri = "https://api.telegram.org/bot" + Token + "/deleteMessage?chat_id=" + (long)chat.TelegramId() + "&message_id=" + (int)m.TelegramId();


                    WebClient c = new WebClient();
                    c.DownloadString(uri);
                }
            }
            catch
            {
                return false;
            }
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

        public override Chat GetChatById(ChatItemId id)
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
            Running = true;
            TClient.StartReceiving();
        }

        private void TClient_OnUpdate(object sender, Telegram.Bot.Args.UpdateEventArgs e)
        {
            Telegram.Bot.Types.Update u = e.Update;
            if (!SupportedTypes.Contains(u.Message.Type)) return;
            string text = u.Message.Text;
            Console.WriteLine("Message received: " + text);
            this.OnMessageReceived(FromTelegramMessage(e.Update.Message));

        }

        public Message FromTelegramMessage(Telegram.Bot.Types.Message message)
        {
            if (message == null) return null;
            ChatItemId id = new ChatItemId("telegram", (ulong)message.MessageId);
            Message m = new Message(this, FromTelegramChat(message.Chat), FromTelegramUser(message.From), message.Text, 
                id, FromTelegramMessageType(message.Type));
            m.voice = FromTelegramVoice(message.Voice);
            m.audio = FromTelegramAudio(message.Audio);

            return m;
        }

        public Audio FromTelegramAudio(Telegram.Bot.Types.Audio v)
        {
            if (v == null) return null;
            
            System.IO.Stream stream = v.FileStream;
            if (stream == null)
            {
                Telegram.Bot.Types.File f = TClient.GetFileAsync(v.FileId).Result;
                stream = f.FileStream;
            }

            Audio audio = new Audio(v.Title, v.Performer, v.Duration, stream);

            return audio;
        }

        public Voice FromTelegramVoice(Telegram.Bot.Types.Voice v)
        {
            if (v == null) return null;
            Voice voice = new Voice(Voice.VoiceAudioType.OggOpus);
            if (v.FileStream == null)
            {
                Telegram.Bot.Types.File f = TClient.GetFileAsync(v.FileId).Result;
                voice.AudioStream = f.FileStream;
            }
            else voice.AudioStream = v.FileStream;
            voice.Duration = v.Duration;
            
            return voice;
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

        public static MessageType FromTelegramMessageType(Telegram.Bot.Types.Enums.MessageType type)
        {
            switch (type)
            {
                case Telegram.Bot.Types.Enums.MessageType.AudioMessage:
                    return MessageType.AudioMessage;
                case Telegram.Bot.Types.Enums.MessageType.ContactMessage:
                    return MessageType.ContactMessage;
                case Telegram.Bot.Types.Enums.MessageType.DocumentMessage:
                    return MessageType.DocumentMessage;
                case Telegram.Bot.Types.Enums.MessageType.LocationMessage:
                    return MessageType.LocationMessage;
                case Telegram.Bot.Types.Enums.MessageType.PhotoMessage:
                    return MessageType.PhotoMessage;
                case Telegram.Bot.Types.Enums.MessageType.StickerMessage:
                    return MessageType.StickerMessage;
                case Telegram.Bot.Types.Enums.MessageType.TextMessage:
                    return MessageType.TextMessage;
                case Telegram.Bot.Types.Enums.MessageType.VoiceMessage:
                    return MessageType.VoiceMessage;
                default:
                    return MessageType.Unknown;
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

        public override User GetChatMemberById(ChatItemId chatId, ulong id)
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
            Running = false;
            TClient.StopReceiving();
        }

        public override string GetClientId()
        {
            return ClientId;
        }
    }
}
