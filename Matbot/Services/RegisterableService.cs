﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace Matbot.Services
{
    /// <summary>
    /// A service that users can register Chats to, allows for services to keep track of Chats.
    /// </summary>
    [Serializable]
    abstract class RegisterableService : Service
    {
        // Fully - a ChatId is fully registered on all clients.
        // Partially - a ChatId is registered on a client.
        // Empty - a ChatId is not registered on any client.
        public enum ChatRegisterStatus
        {
            Fully,
            Partially,
            Empty
        }

        public Client.UserRank RequiredRank = Client.UserRank.User;

        protected List<ChatItemId> Registered = new List<ChatItemId>();

        public override string ToString(Matbot.Client.ChatItemId chatId)
        {
            string s = base.ToString();

            if (IsChatRegistered(chatId) != ChatRegisterStatus.Empty) s += "    *R*";
            else s += "    R";

            return s;
        }

        public RegisterableService(Bot bot) : base(bot)
        {
        }

        public RegisterableService(Bot bot, string desc) : base(bot,desc)
        {
        }

        public ChatItemId[] Chats
        {
            get
            {
                return Registered.ToArray();
            }
        }

        protected ChatRegisterStatus IsChatRegistered(Chat c)
        {
            return IsChatRegistered(c.Id);
        }

        protected ChatRegisterStatus IsChatRegistered(ChatItemId c)
        {

            foreach(ChatItemId chat in Registered)
            {
                if (chat.Equals(c)) return ChatRegisterStatus.Fully;
                else if (chat.Intersects(c)) return ChatRegisterStatus.Partially;
            }

            return ChatRegisterStatus.Empty;
        }

        public void Unregister(Chat c)
        {
            Unregister(c.Id);
        }

        public void Unregister(ChatItemId c)
        {
            foreach (ChatItemId chat in Registered)
            {
                if (c.Intersects(chat))
                {
                    Registered.Remove(chat);
                    return;
                }
            }
            throw new Exceptions.UserNotRegisteredException(this);
        }


        public void Register(Chat c)
        {
            Register(c.Id);
        }

        public void Register(ChatItemId c)
        {
            ChatRegisterStatus status = IsChatRegistered(c);
            if (status == ChatRegisterStatus.Fully) throw new Exceptions.UserAlreadyRegisteredException(this);
            else if (status == ChatRegisterStatus.Partially)
            {
                foreach (ChatItemId chat in Registered)
                {
                    if (chat.Intersects(c))
                    {
                        Registered.Remove(chat);
                        Registered.Add(ChatItemId.Merge(c, chat));
                    }
                }
            }
            else
            {
                Registered.Add(c);
            }
        }
    }
}
