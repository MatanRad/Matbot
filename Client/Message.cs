using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Client
{
    public enum MessageType
    {
        TextMessage,
        AudioMessage,
        VoiceMessage,
        DocumentMessage,
        PhotoMessage,
        StickerMessage,
        LocationMessage
    }


    public class Message
    {
        public Chat Chat { get; private set; }
        public User User { get; private set; }
        public string Text { get; private set; }
        public Message AReplyTo;

        public MessageType Type { get; private set; }


        private Client Client = null;

        public Message(Chat chat, User user, string text)
        {
            Chat = chat;
            User = user;
            Text = text;
        }

        public Message(Chat chat, User user, string text, Client client)
        {
            Chat = chat;
            User = user;
            Text = text;
            Client = client;
        }

        public bool Reply(string message)
        {
            return this.Client.SendMessage(this.Chat, message);
        }

        public bool Reply(FormattedMessage message)
        {
            return this.Client.SendMessage(this.Chat, message);
        }
        
    }
}
