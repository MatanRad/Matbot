using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;

namespace Matbot.Commands
{
    class RegisterCommand : SingleStringCommand
    {
        private static string password = "potato";

        public RegisterCommand() : base("register")
        {
            HasRequiredRank = false;
        }

        public override void Execute(Message message)
        {
            VerifyContext(message);
        }

        private bool VerifyContext(Message m)
        {
            if(!m.Chat.IsPrivate())
            {
                m.Reply("You can only use this command in a private chat!");
                return false;
            }

            if(m.User.BotRank>=UserRank.Owner)
            {
                m.Reply("You are already an owner!");
                return false;
            }

            return true;
        }

        public void Execute(Message message, string s)
        {
            if (!VerifyContext(message)) return;
            if (password.Equals(s))
            {
                message.User.ChangeDatabaseRank(UserRank.Owner);
                message.Reply("You are now an owner.");
            }
            else
            {
                message.Reply("Invalid Password.");
            }
        }
    }
}
