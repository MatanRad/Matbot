using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Client
{
    [Serializable]
    public class ChatItemId
    {
        public Dictionary<string, ulong> Ids = new Dictionary<string, ulong>();

        private ChatItemId() { }

        public ulong this[string key]
        {
            get
            {
                return Ids[key];
            }

            set
            {
                Ids[key] = value;
            }
        }


        public ChatItemId(string clientId, ulong id)
        {
            Ids.Add(clientId, id);
        }

        /// <summary>
        /// Check if the 2 IDS have an itersection, that means if there exists some client in which both have the same ID.
        /// </summary>
        /// <param name="other">The other ChatID</param>
        public bool Intersects(ChatItemId other)
        {
            foreach(KeyValuePair<string,ulong> p in Ids)
            {
                if (other.Ids.ContainsKey(p.Key)) if (other.Ids[p.Key] == Ids[p.Key]) return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if there is a conflict in the chatids, meaning a client in which they have different IDs.
        /// </summary>
        public bool Conflicts(ChatItemId other)
        {
            foreach (KeyValuePair<string, ulong> p in Ids)
            {
                if (other.Ids.ContainsKey(p.Key)) if (other.Ids[p.Key] != Ids[p.Key]) return true;
            }

            return false;
        }

        public static ChatItemId Merge(ChatItemId c1, ChatItemId c2)
        {
            if (c1 == null || c2 == null) return null;
            if (c1.Conflicts(c2)) return null;

            ChatItemId id = new ChatItemId();

            foreach (KeyValuePair<string, ulong> p in c1.Ids)
            {
                id.Ids.Add(p.Key, p.Value);
            }

            foreach (KeyValuePair<string, ulong> p in c2.Ids)
            {
                if(!id.Ids.ContainsKey(p.Key)) id.Ids.Add(p.Key, p.Value);
            }

            return id;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChatItemId)) return false;

            ChatItemId other = obj as ChatItemId;
            foreach (KeyValuePair<string, ulong> p in Ids)
            {
                if (!other.Ids.Contains(p)) return false;
            }

            foreach (KeyValuePair<string, ulong> p in other.Ids)
            {
                if (!Ids.Contains(p)) return false;
            }

            return true;
        }

        public ChatItemId(Dictionary<string, ulong> ids)
        {
            Ids = new Dictionary<string, ulong>(ids);
        }

        public override string ToString()
        {
            string s = "{";
            foreach (KeyValuePair<string, ulong> kv in Ids)
            {
                s += kv.Key + " : " + kv.Value.ToString() + ", ";
            }
            s += "};";

            return s;
        }
    }
}
