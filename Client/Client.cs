using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Commands.Structure;

namespace Matbot.Client
{
    public abstract class Client
    {
        CommandManager CmdManger = new CommandManager();
        UserDatabase usrDatabase = new UserDatabase();

        public UserDatabase UsrDatabase { get { return usrDatabase; } private set { usrDatabase = value; } }

        public abstract void Start();
        public abstract void Stop();

        public abstract string GetClientId();

        public virtual void OnMessageReceived(Message message)
        {
            ParsedInput parsed = new ParsedInput(message.Text);

            this.CmdManger.ExecuteUserInput(message, parsed);
        }

        public void Initialize()
        {
            Matbot.Commands.PlayCommand cmd = new Commands.PlayCommand();
            Matbot.Commands.RegisterCommand cm2 = new Commands.RegisterCommand();
            Matbot.Commands.MyRankCommand cm3 = new Commands.MyRankCommand();
            Matbot.Commands.MyIdCommand cm4 = new Commands.MyIdCommand();
            Matbot.Commands.UpgradeCommand cm5 = new Commands.UpgradeCommand();
            Matbot.Commands.ACCommand cm6 = new Commands.ACCommand();
            Matbot.Commands.ACComCommand cm7 = new Commands.ACComCommand();
            Matbot.Commands.SongCommand cm8 = new Commands.SongCommand();
            CommandManager.RegisterNewCommand(cmd);
            CommandManager.RegisterNewCommand(cm2);
            CommandManager.RegisterNewCommand(cm3);
            CommandManager.RegisterNewCommand(cm4);
            CommandManager.RegisterNewCommand(cm5);
            CommandManager.RegisterNewCommand(cm6);
            CommandManager.RegisterNewCommand(cm7);
            CommandManager.RegisterNewCommand(cm8);
        }

        public Client()
        {
            Initialize();
        }

        public Client(UserDatabase database)
        {
            UsrDatabase = database;
            Initialize();
        }

        public virtual void SetUserRank(User user, UserRank rank)
        {
            UsrDatabase.SetUserRank(user, rank);
        }

        public virtual Chat GetChatById(ChatId id)
        {
            return null;
        }

        public virtual bool SendMessage(ChatId id, string message)
        {
            Chat c = GetChatById(id);
            if (c == null) c= new Chat(id, ChatType.Unknown);

            return this.SendMessage(c, message);
        }

        public virtual bool SendMessage(ChatId id, FormattedMessage message)
        {
            Chat c = GetChatById(id);
            if (c == null) c = new Chat(id, ChatType.Unknown);

            return this.SendMessage(c, message);
        }

        public abstract bool SendMessage(Chat chat, string message);

        public virtual bool SendMessage(Chat chat, FormattedMessage message)
        {
            throw new NotImplementedException("Reply for FormattedMessage not yet implemented");
        }

        public virtual bool SendAudioMessage(Chat chat, Audio audio)
        {
            return false;
        }

        public virtual User GetNewUser(ulong id)
        {
            User u = new User(this, this.GetClientId(), id);
            try
            {
                u.BotRank = UsrDatabase.GetUserRank(u);
            }
            catch(Exceptions.UserNotFoundException) { }


            return u;
        }

        public virtual User GetChatMember(ChatId chatId,ulong id)
        {
            return null;
        }
    }
}
