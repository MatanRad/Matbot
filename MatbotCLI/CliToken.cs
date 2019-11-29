using Matbot.Client;

namespace MatbotCLI
{
    public class CliToken : ClientToken
    {
        public CliToken() : base(typeof(CliClient), "")
        { }
    }
}
