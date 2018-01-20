using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Client
{

    public enum ChatType : uint
    {
        Unknown,
        Channel,
        Group,
        Private,
        Supergroup
    }


    public class Chat
    {
        public ChatItemId Id { get; private set; }
        public string Title { get; set; }

        public ChatType Type { get; set; }

        public virtual bool IsPrivate()
        {
            return Type == ChatType.Private;
        }

        public Chat(string clientId, ulong id, ChatType type)
        {
            Type = type;
            Id = new ChatItemId(clientId, id);
        }

        public Chat(ChatItemId id, ChatType type)
        {
            Type = type;
            Id = id;
        }
    }
}
