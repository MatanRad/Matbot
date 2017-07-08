using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matbot.Client
{
    public class ClientToken
    {
        public readonly Type ClientType;
        public readonly string Token;

        public ClientToken(Type clientType, string token)
        {
            ClientType = clientType;
            Token = token;
        }
    }
}
