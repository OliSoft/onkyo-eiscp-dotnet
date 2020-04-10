namespace Eiscp.Core.Commands
{
    /// <summary>
    /// On/Off Command
    /// </summary>
    public class OnOffCommand : BaseCommand
    {
        /// <summary>
        /// On
        /// </summary>
        public string On => GetCommandString("01");
        /// <summary>
        /// Off
        /// </summary>
        public string Off => GetCommandString("00");
    }
}
