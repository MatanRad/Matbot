using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Matbot.Client
{
    [Serializable]
    public class UserDatabase
    {
        List<User> users = new List<User>();

        public UserDatabase()
        {
            /*XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
            using (var sww = new StreamReader("users.xml"))
            {
                using (XmlReader reader = XmlReader.Create(sww))
                {
                    users = serializer.Deserialize(reader) as List<User>;
                }
            }*/
            try
            {
                using (var ms = new FileStream("users.xml", FileMode.Open))
                {
                    var set = new BinaryFormatter();
                    users = set.Deserialize(ms) as List<User>;
                }
            }
            catch (FileNotFoundException) { }
            catch (SerializationException) { }
            
            
        }

        public User FindUserById(string client, ulong id, Client c = null)
        {
            int i = FindUserIndexById(client, id);
            if (i != -1) users[i].SetClient(c);
            return (i==-1 ? null : users[i]);
        }

        private int FindUserIndexById(string client, ulong id, Client c = null)
        {
            for(int i=0;i<users.Count;i++)
            {
                User u = users[i];
                foreach (KeyValuePair<string, ulong> e in u.Id.Ids)
                {
                    if (client.Equals(e.Key) && id.Equals(e.Value))
                    {
                        u.SetClient(c);
                        return i;
                    }
                }
            }
            return -1;
        }

        public bool UserExists(string client, ulong id)
        {
            return FindUserIndexById(client, id)!=-1;
        }

        public User FindUserById(ChatId id, Client c = null)
        {
            int i = FindUserIndexById(id);
            if (i != -1) users[i].SetClient(c);
            return (i == -1 ? null : users[i]);
        }

        private int FindUserIndexById(ChatId id, Client c = null)
        {
            foreach (KeyValuePair<string, ulong> e in id.Ids)
            {
                int i = FindUserIndexById(e.Key, e.Value);
                if (i != -1)
                {
                    users[i].SetClient(c);
                    return i;
                }
            }

            return -1;
        }

        public bool UserExists(ChatId id)
        {
            return FindUserIndexById(id) != -1;
        }

        public void AddUser(User user)
        {
            if (UserExists(user.Id)) throw new Matbot.Client.Exceptions.UserAlreadyExistsException(user);

            users.Add(user);

            SaveChanges();
        }

        public void RemoveUser(User user)
        {
            int i = FindUserIndexById(user.Id);
            if (i == -1) throw new Matbot.Client.Exceptions.UserNotFoundException(user);

            users.RemoveAt(i);

            SaveChanges();
        }

        public void SetUserRank(User user, UserRank rank)
        {
            User u = FindUserById(user.Id);
            if (u == null)
            {
                AddUser(user);
                u = FindUserById(user.Id);
            }
            user.BotRank = rank;
            u.BotRank = rank;

            SaveChanges();
        }

        public UserRank GetUserRank(User user)
        {
            User u = FindUserById(user.Id);
            if (u == null) throw new Matbot.Client.Exceptions.UserNotFoundException(user);
            return u.BotRank;
        }

        private void SaveChanges()
        {
            /*XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
            using (var sww = new StreamWriter("users.xml"))
            {
                using (XmlWriter writer = XmlWriter.Create(sww))
                {
                    serializer.Serialize(writer, users);
                }
            }*/

            using (var ms = new FileStream("users.xml", FileMode.Create))
            {
                var set = new BinaryFormatter();
                set.Serialize(ms, users);
            }
        }


    }
}
