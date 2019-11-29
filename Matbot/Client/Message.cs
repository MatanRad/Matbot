using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Client
{
    /// <summary>
    /// Represents a type of message, with respect to big messaging apps.
    /// </summary>
    public enum MessageType
    {
        TextMessage,
        AudioMessage,
        VoiceMessage,
        DocumentMessage,
        PhotoMessage,
        StickerMessage,
        LocationMessage,
        ContactMessage,
        Unknown
    }

    /// <summary>
    /// A wrapper for a chat message.
    /// Main form of communication with the bot.
    /// </summary>
    public class Message
    {
        public ChatItemId Id;

        /// <summary>
        /// The chat this message was sent in.
        /// </summary>
        public Chat Chat { get; private set; }

        /// <summary>
        /// Author of the message.
        /// </summary>
        public User User { get; private set; }
        public string Text { get; private set; }
        public Audio audio = null;
        public Voice voice = null;
        public Message AReplyTo;

        public MessageType Type { get; private set; }


        public readonly Client Client = null;

        public Message(Chat chat, User user, string text, ChatItemId id=null, MessageType type = MessageType.Unknown)
        {
            Chat = chat;
            User = user;
            Text = text;
            Id = id;
            Type = type;
        }

        public Message(Client client, Chat chat, User user, string text, ChatItemId id = null, MessageType type = MessageType.Unknown)
        {
            Chat = chat;
            User = user;
            Text = text;
            Client = client;
            Id = id;
            Type = type;
        }

        /// <summary>
        /// Send text message in this message's chat.
        /// </summary>
        /// <returns>Success of delivery.</returns>
        public bool Reply(string message)
        {
            return this.Client.SendMessage(this.Chat, message);
        }

        /// <summary>
        /// Send a formatted message in this message's chat.
        /// </summary>
        /// <returns>Success of delivery.</returns>
        public bool Reply(FormattedMessage message)
        {
            return this.Client.SendMessage(this.Chat, message);
        }
        
    }
}
