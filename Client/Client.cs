using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Matbot.Commands.Structure;
using Matbot.Handlers.Structure;
namespace Matbot.Client
{
    public abstract class Client
    {
        public Bot Bot;

        protected CommandManager CustomCmdManger = new CommandManager();
        protected HandlerManager CustomHndManager = new HandlerManager();

        public bool Running = false;

        public abstract void Start();
        public abstract void Stop();

        public abstract string GetClientId();

        public virtual void RegisterHandler(IHandler h)
        {
            CustomHndManager.Register(h);
        }

        public CommandDescriptor[] GetCommandDescriptors()
        {
            CommandDescriptor[] clientCommands = this.CustomCmdManger.GetCommandDescriptors();
            CommandDescriptor[] botCommands = this.Bot.SharedManager.GetCommandDescriptors();

            if (clientCommands == null) return botCommands;

            CommandDescriptor[] finArray = new CommandDescriptor[clientCommands.Length + botCommands.Length];

            Array.Copy(clientCommands, finArray, clientCommands.Length);
            Array.Copy(botCommands, 0, finArray, clientCommands.Length, botCommands.Length);

            return finArray;
        }

        public virtual void OnMessageReceived(Message message)
        {
            if (message.Type == MessageType.TextMessage)
            {
                ParsedInput parsed = new ParsedInput(message.Text);
                if (parsed.IsCommand)
                {
                    if (!this.CustomCmdManger.ExecuteUserInput(message, parsed))
                    {
                        Bot.SharedManager.ExecuteUserInput(message, parsed);
                    }
                    return;
                }
            }
            
            CustomHndManager.MessageReceieved(message);
            Bot.SharedHndManager.MessageReceieved(message);
            
        }

        public List<IHandler> GetHandlers()
        {
            return CustomHndManager.handlers;
        }

        public Client(Bot bot, ClientToken token)
        {
            this.Bot = bot;
        }

        public virtual void SetUserRank(User user, UserRank rank)
        {
            Bot.UsrDatabase.SetUserRank(user, rank);
        }

        public virtual Chat GetChatById(ChatItemId id)
        {
            throw new NotImplementedException("GetChatById functionality not implemented in bot client with id: " + GetClientId());
        }

        public virtual bool SendMessage(ChatItemId id, string message)
        {
            Chat c = GetChatById(id);
            if (c == null) c= new Chat(id, ChatType.Unknown);

            return this.SendMessage(c, message);
        }

        public virtual bool SendMessage(ChatItemId id, FormattedMessage message)
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

        public virtual bool DeleteMessage(Message m)
        {
            throw new NotImplementedException("DeleteMessage functionality not implemented in bot client with id: " + GetClientId());
        }

        public virtual User GetNewUser(ulong id)
        {
            User u = new User(this, this.GetClientId(), id);
            try
            {
                u.BotRank = Bot.UsrDatabase.GetUserRank(u);
            }
            catch(Exceptions.UserNotFoundException) { }


            return u;
        }

        public virtual User GetChatMemberById(ChatItemId chatId,ulong id)
        {
            throw new NotImplementedException("GetChatMember functionality not implemented in bot client with id: " + GetClientId());
        }

        public virtual User GetChatMemberByUsername(ChatItemId chatId, string username, bool exactMatch = true)
        {
            throw new NotImplementedException("GetChatMemberByUsername functionality not implemented in bot client with id: " + GetClientId());
        }
    }
}
