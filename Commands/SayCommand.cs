using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;
using Matbot.Commands.Structure;

namespace Matbot.Commands
{
    class SayCommand : Structure.Command
    {
        public SayCommand() : base("say")
        {
            this.RequiredRank = UserRank.Admin;
        }

        public override void Execute(Message message)
        {
            
        }

        public override void ReformatInput(ParsedInput input)
        {
            if (input.Parameters.Length > 1)
            {
                string[] s = new string[2];
                s[0] = input.Parameters[0];
                s[1] = "";

                for (int i = 1; i < input.Parameters.Length; i++)
                {
                    s[1] += input.Parameters[i] + " ";
                }

                input.Parameters = s;
            }
        }

        public void Execute(Message message, ulong chatid, string msg)
        {
            Chat c = message.Client.GetChatById(new ChatId(message.Client.GetClientId(), chatid));

            if(c!=null)
            {
                message.Client.SendMessage(c, msg);
            }
        }
    }
}
