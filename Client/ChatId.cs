using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Client
{
    [Serializable]
    public class ChatId
    {
        public Dictionary<string, ulong> Ids = new Dictionary<string, ulong>();

        private ChatId() { }

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


        public ChatId(string clientId, ulong id)
        {
            Ids.Add(clientId, id);
        }

        public ChatId(Dictionary<string, ulong> ids)
        {
            Ids = new Dictionary<string, ulong>(ids);
        }
    }
}
