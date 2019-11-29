using Matbot.Client;

namespace Matbot.Handlers.Structure
{
    /// <summary>
    /// A message handler interface.
    /// </summary>
    public interface IHandler
    {
        /// <summary>
        /// Was this handler killed.
        /// </summary>
        bool IsEnabled();

        /// <summary>
        /// Returns that given a message, should this handler run on it.
        /// </summary>
        /// <param name="m">Message to handle.</param>
        bool ShouldHandle(Message m);

        /// <summary>
        /// Run on the received message.
        /// </summary>
        /// <param name="m">Message to handle.</param>
        void Handle(Message m);

        /// <summary>
        /// Stop the handler.
        /// </summary>
        void Stop();
    }
}
