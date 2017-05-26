using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Client
{
    public enum UserRank
    {
        Gali = -2,
        Banned = -1,
        User = 0,
        Admin = 1,
        Owner = 2
    }

    public class User
    {
        public ChatId Id { get; private set; }
        public string Username;


        public string Name;
        public UserRank BotRank = UserRank.User;

        public User(string clientId, ulong id)
        {
            Id = new ChatId(clientId, id);
        }

    }
}
