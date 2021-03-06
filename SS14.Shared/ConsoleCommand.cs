// Basic abstract to handle console commands.
// Note that there is no Execute() function, this is due to chat & client commands needing a client,
// While client-side and server console commands don't.
namespace SS14.Shared.Command
{
    public interface ICommand
    {
        /// <summary>
        /// Name of the command.
        /// </summary>
        /// <value>
        /// A string as indentifier for this command.
        /// </value>
        string Command { get; }

        /// <summary>
        /// Short description of the command.
        /// </summary>
        /// <value>
        /// String printed as short summary in the "help" command.
        /// </value>
        string Description { get; }

        /// <summary>
        /// Extended description for the command.
        /// </summary>
        /// <value>
        /// String printed as summary when "help Command" is used.
        /// </value>
        string Help { get; }

        /*
        /// <summary>
        /// Runs the command.
        /// </summary>
        /// <param name="args">Additional arguments to pass to the command.</param>
        void Execute(params string[] args);
        */
    }
}