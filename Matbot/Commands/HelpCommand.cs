using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Client;
using Matbot.Commands.Structure;

namespace Matbot.Commands
{
    class HelpCommand : Command
    {
        private static int itemsPerPage = 10;
        private static string moreInfoStr = "For more info about /help command type: \"/help help\"";

        public HelpCommand() : base("help")
        {
            this.paramTypePriorities = new Dictionary<Type, int>
            {
                { typeof(int), 1 }
            };
        }

        private string GetCommandsString(CommandDescriptor[] descs, int page)
        {
            string s = "";

            int pageEnd = Math.Min(descs.Length, (page + 1) * itemsPerPage);

            for (int i = page * itemsPerPage; i < pageEnd; i++)
            {
                s += "/" + descs[i].Name;
                if (i != pageEnd - 1) s += "\n";
            }

            return s;
        }

        public override void Execute(Message message)
        {
            Execute(message, 1);
        }

        public void Execute(Message m, int pageNumber)
        {
            var descs = m.Client.GetCommandDescriptors();
            int totalPageNum = (int)Math.Ceiling((float)descs.Length / itemsPerPage);

            if (pageNumber < 1 || pageNumber > totalPageNum)
            {
                if (totalPageNum <= 1)
                    m.Reply("Page " + pageNumber + " out of index! Must be between 1!\n" + moreInfoStr);
                else
                    m.Reply("Page " + pageNumber + " out of index! Must be between 1 and " + totalPageNum + "!\n" + moreInfoStr);

                return;
            }

            string msg = moreInfoStr + "\nCommands (page " + pageNumber +"/" + totalPageNum + "):\n";

            msg += GetCommandsString(descs, pageNumber - 1);
            m.Reply(msg);
        }

        public void Execute(Message m, string cmdName)
        {
            CommandDescriptor cmd;
            CommandDescriptor[] descs = m.Client.GetCommandDescriptors();

            foreach (CommandDescriptor cd in descs)
            {
                if(cd.Name.Equals(cmdName))
                {
                    m.Reply("Variations of /" + cmdName + " command:\n" + cd.ToStringDetailed());
                    return;
                }
            }

            m.Reply("Command /" + cmdName + " doesn't exist!\nFor command list type: \"/help\".");
        }
    }
}
