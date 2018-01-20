﻿using System;
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
        LocationMessage,
        ContactMessage,
        Unknown
    }


    public class Message
    {
        public ChatItemId Id;
        public Chat Chat { get; private set; }
        public User User { get; private set; }
        public string Text { get; private set; }
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
