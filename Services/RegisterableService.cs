using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace Matbot.Services
{
    [Serializable]
    abstract class RegisterableService : Service
    {
        public enum ChatRegisterStatus
        {
            Fully,
            Partially,
            Empty
        }

        public Client.UserRank RequiredRank = Client.UserRank.User;

        protected List<ChatId> Registered = new List<ChatId>();

        public RegisterableService(Bot bot) : base(bot)
        {
        }

        public ChatId[] Chats
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

        protected ChatRegisterStatus IsChatRegistered(ChatId c)
        {

            foreach(ChatId chat in Registered)
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

        public void Unregister(ChatId c)
        {
            foreach (ChatId chat in Registered)
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

        public void Register(ChatId c)
        {
            ChatRegisterStatus status = IsChatRegistered(c);
            if (status == ChatRegisterStatus.Fully) throw new Exceptions.UserAlreadyRegisteredException(this);
            else if (status == ChatRegisterStatus.Partially)
            {
                foreach (ChatId chat in Registered)
                {
                    if (chat.Intersects(c))
                    {
                        Registered.Remove(chat);
                        Registered.Add(ChatId.Merge(c, chat));
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
