using Eiscp.Core.Helper;
using Eiscp.Core.Model;
using System.Collections.Specialized;

namespace Eiscp.Core.Commands
{
    public abstract class BaseCommand
    { 
        public bool IsSelected { get; set; }

        public virtual string Key { get; }
        public virtual OrderedDictionary Value { get; }
        public Response Response { get; protected set; }

        public string Query => GetCommandString("QSTN");

        protected string GetCommandString(string arg)
        {
            var name = (string)Utils.Nav(Value, "name");
            var cmd = (string)Utils.Nav(Value, "values", arg, "name");
            return $"{name}.{cmd}";
        }

        public bool IsCommand(string response)
        {
            Response = new Response(response, Value);
            return Response.Key.Equals(Key);
        } 
    }
}
