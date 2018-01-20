using Matbot.Client;

namespace Matbot.Handlers.Structure
{
    public interface IHandler
    {
        bool IsEnabled();

        bool ShouldHandle(Message m);

        void Handle(Message m);

        void Stop();
    }
}
