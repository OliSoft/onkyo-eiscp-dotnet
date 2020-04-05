using System;
using System.Collections.Generic;
using System.Text;

namespace Eiscp.Core.Commands
{
    public class OnOffCommand : BaseCommand
    {
        public string On => GetCommandString("01");
        public string Off => GetCommandString("00");
    }
}
