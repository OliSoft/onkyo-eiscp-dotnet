using Eiscp.Core.Helper;
using Eiscp.Core.Model;
using System.Collections.Specialized;

namespace Eiscp.Core.Commands
{
    /// <summary>
    /// Base Command
    /// </summary>
    public abstract class BaseCommand
    { 
        /// <summary>
        /// Is selected
        /// </summary>
        public bool IsSelected { get; set; }
        /// <summary>
        /// Key
        /// </summary>
        public virtual string Key { get; }
        /// <summary>
        /// Value
        /// </summary>
        public virtual OrderedDictionary Value { get; }
        /// <summary>
        /// Response
        /// </summary>
        public Response Response { get; protected set; }
        /// <summary>
        /// Query
        /// </summary>
        public string Query => GetCommandString("QSTN");
        /// <summary>
        /// Command String
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        protected string GetCommandString(string arg)
        {
            var name = (string)Utils.Nav(Value, "name");
            var cmd = (string)Utils.Nav(Value, "values", arg, "name");
            return $"{name}.{cmd}";
        }
        /// <summary>
        /// Is command
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public bool IsCommand(string response)
        {
            Response = new Response(response, Value);
            return Response.Key.Equals(Key);
        } 
    }
}
