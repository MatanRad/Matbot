using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Client
{
    [Serializable]
    public enum UserRank
    {
        Gali = -2,
        Banned = -1,
        User = 0,
        Admin = 1,
        Owner = 2
    }

    /// <summary>
    /// Wrapper for a bot user.
    /// </summary>
    [Serializable]
    public class User
    {
        public ChatItemId Id { get; private set; }
        public string Username;

        [NonSerialized]
        private Client Client = null;

        public string Name;
        public UserRank BotRank = UserRank.User;

        public User(string clientId, ulong id)
        {
            Id = new ChatItemId(clientId, id);
        }

        private User() { }

        public void SetClient(Client c)
        {
            if (Client == null) Client = c;
        }

        public User(Client c, string clientId, ulong id)
        {
            Id = new ChatItemId(clientId, id);
            Client = c;
        }

        /// <summary>
        /// Update a User's rank.
        /// </summary>
        public void ChangeDatabaseRank(UserRank rank)
        {
            Client.SetUserRank(this, rank);
        }

        /// <summary>
        /// Return string describing user id and name.
        /// </summary>
        public override string ToString()
        {
            return "User " + Id.Ids.ToString() + "\nName: " + Name;
        }

    }
}
