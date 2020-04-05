namespace Eiscp.Core.Commands
{
    public abstract class UpDownCommand : BaseCommand
    {
        public string UP => GetCommandString("UP");
        public string DOWN => GetCommandString("DOWN");
    }
}
