using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;
using System.Reflection;

namespace Matbot
{
    public class Bot
    {
        public Commands.Structure.CommandManager SharedManager = new Commands.Structure.CommandManager();
        public Handlers.Structure.HandlerManager SharedHndManager = new Handlers.Structure.HandlerManager();
        public UserDatabase UsrDatabase = new UserDatabase("users.dat");
        public Services.ServiceManager SrvManager;

        public Dictionary<string, Client.Client> Clients = new Dictionary<string, Client.Client>();

        public bool AddClient(ClientToken token)
        {
            if (!token.ClientType.IsSubclassOf(typeof(Client.Client))) return false;

            // get public constructors
            ConstructorInfo[] ctors = token.ClientType.GetConstructors();

            // invoke the first public constructor with no parameters.
            foreach(ConstructorInfo inf in ctors)
            {
                if (!inf.IsPublic) continue;
                var param = inf.GetParameters();
                if(param.Length==2)
                {
                    if (param[0].ParameterType.Equals(typeof(Bot)))
                    {
                        if(param[1].ParameterType.Equals(token.GetType()) || token.GetType().IsSubclassOf(param[1].ParameterType))
                        {
                            Client.Client client = (Client.Client)inf.Invoke(new object[] { this, token });
                            client.Start();
                            Clients.Add(client.GetClientId(), client);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void StopAll()
        {
            foreach(KeyValuePair<string, Client.Client> p in Clients)
            {
                p.Value.Stop();
            }
        }

        public Bot()
        {
            SrvManager = new Services.ServiceManager(this, "services.dat");
            Commands.Structure.CommandAllocator.AllocateShared(SharedManager);
        }

        public void BroadcastMessage(ChatItemId id, string message)
        {
            foreach(KeyValuePair<string, ulong> p in id.Ids)
            {
                if (Clients.ContainsKey(p.Key)) Clients[p.Key].SendMessage(id, message);
            }
        }
    }
}
