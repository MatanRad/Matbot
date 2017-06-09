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
        CommandManager CustomCmdManger = new CommandManager();
        UserDatabase usrDatabase = new UserDatabase();

        List<Command> CustomCommands = new List<Command>();

        public UserDatabase UsrDatabase { get { return usrDatabase; } private set { usrDatabase = value; } }

        public abstract void Start();
        public abstract void Stop();

        public abstract string GetClientId();

        public virtual void OnMessageReceived(Message message)
        {
            ParsedInput parsed = new ParsedInput(message.Text);

            if(!this.CustomCmdManger.ExecuteUserInput(message, parsed))
            {
                CommandManager.SharedManager.ExecuteUserInput(message, parsed);
            }
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
            Matbot.Commands.BotConsoleCommand cm9 = new Commands.BotConsoleCommand();
            Matbot.Commands.AlarmCommand cm10 = new Commands.AlarmCommand();
            Matbot.Commands.AddCommand cm11 = new Commands.AddCommand();
            Matbot.Commands.FindUserCommand cm12 = new Commands.FindUserCommand();
            CommandManager.SharedManager.RegisterNewCommand(cmd);
            CommandManager.SharedManager.RegisterNewCommand(cm2);
            CommandManager.SharedManager.RegisterNewCommand(cm3);
            CommandManager.SharedManager.RegisterNewCommand(cm4);
            CommandManager.SharedManager.RegisterNewCommand(cm5);
            CommandManager.SharedManager.RegisterNewCommand(cm6);
            CommandManager.SharedManager.RegisterNewCommand(cm7);
            CommandManager.SharedManager.RegisterNewCommand(cm8);
            CommandManager.SharedManager.RegisterNewCommand(cm9);
            CommandManager.SharedManager.RegisterNewCommand(cm10);
            CommandManager.SharedManager.RegisterNewCommand(cm11);
            CommandManager.SharedManager.RegisterNewCommand(cm12);

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
            throw new NotImplementedException("GetChatById functionality not implemented in bot client with id: " + GetClientId());
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
            throw new NotImplementedException("SendAudioMessage functionality not implemented in bot client with id: " + GetClientId());
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

        public virtual User GetChatMemberById(ChatId chatId,ulong id)
        {
            throw new NotImplementedException("GetChatMember functionality not implemented in bot client with id: " + GetClientId());
        }

        public virtual User GetChatMemberByUsername(ChatId chatId, string username, bool exactMatch = true)
        {
            throw new NotImplementedException("GetChatMemberByUsername functionality not implemented in bot client with id: " + GetClientId());
        }
    }
}
