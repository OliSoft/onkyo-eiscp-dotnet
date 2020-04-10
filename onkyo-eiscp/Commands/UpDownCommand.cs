namespace Eiscp.Core.Commands
{
    /// <summary>
    /// Up/Down Command
    /// </summary>
    public abstract class UpDownCommand : BaseCommand
    {
        /// <summary>
        /// Up
        /// </summary>
        public string UP => GetCommandString("UP");
        /// <summary>
        /// Down
        /// </summary>
        public string DOWN => GetCommandString("DOWN");
    }
}
