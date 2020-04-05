using Eiscp.Core.Commands;
using Eiscp.Core.Message;
using Eiscp.Core.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eiscp.Core.Helper
{
    public static class Utils
    {
        /// <summary>
        /// Convert an ascii command like (PVR00) to the binary data we
        /// need to send to the receiver.
        /// </summary>        
        public static EiscpPacket CommandToPacket(string command)
        {
            return new EiscpPacket(new IscpMessage(command));
        }

        /// <summary>
        /// Ensures that various ways to refer to a command can be used.
        /// </summary>
        public static string NormalizeCommand(string command)
        {
            return command.ToLower().Replace('_', ' ').Replace('-', ' ');
        }


        /// <summary>
        /// Navigate a dictionary tree.
        /// </summary>
        /// This function saves the tedium of repeated casts when someone wants to
        /// retrieve an object located in a deeply nested dictionary.
        /// <param name="dict">Root of a dictionary tree.</param>
        /// <param name="keys">Path to the object in the tree.</param>
        /// <returns>Node pointed by the path. It may be itself a dictionary.</returns>
        public static object Nav(object dict, params object[] keys)
        {
            foreach (object key in keys)
            {
                if (dict == null)
                    return null;
                dict = (dict as IDictionary)[key];
            }

            return dict;
        }

        /// <summary>
        /// Transform the given given high-level command to a
        /// low-level ISCP message.
        /// </summary>
        ///
        /// <exception cref="ArgumentException">
        /// Raises <c>ArgumentException</c> if <paramref name="command"/> is not valid.
        /// </exception>
        /// 
        /// This exposes a system of human-readable, "pretty"
        /// commands, which is organized into three parts: the zone, the
        /// command, and arguments. For example:    
        /// 
        /// <example>
        /// <code>
        ///     Command("power", "on");
        ///     Command("power", "on", zone: "main");
        ///     Command("volume", "66", zone: "zone2");
        /// </code>
        /// </example>
        /// 
        /// As you can see, if no zone is given, the main zone is assumed.
        ///
        /// Instead of passing three different parameters, you may put the
        /// whole thing in a single string, which is helpful when taking
        /// input from users:
        ///
        /// <example>
        /// <code>
        ///    Command("power on:);
        ///    Command("zone2 volume 66");
        /// </code>
        /// </example>
        /// To further simplify things, for example when taking user input
        /// from a command line, where whitespace needs escaping, the
        /// following is also supported:
        ///
        /// <example>
        /// <code>
        ///     Command("power=on");
        ///     Command("zone2.volume=66");
        /// </code>
        /// </example>
        public static string CommandToIscp(string command, string arguments = null, string zone = null)
        {
            List<string> argumentsList = null;
            string defaultZone = "main";
            char[] commandSep = new char[] { '.', ' ' };
            string norm(string s) => s.Trim().ToLower();

            // If parts are not explicitly given, parse the command
            if (arguments == null && zone == null)
            {
                // Separating command and args with colon allows multiple args
                if (command.Contains(":") || command.Contains("="))
                {
                    char[] separators = new char[] { ':', '=' };
                    string[] baseAndArguments = command.Split(separators, 2); // in Python counterpart it's "max 1 split", here - it's "max 2 parts"
                    string commandBase = baseAndArguments[0];
                    string commandArguments = baseAndArguments[1];

                    var parts = new List<string>(
                        from c in commandBase.Split(commandSep)
                        select norm(c)
                    );

                    if (parts.Count == 2)
                    {
                        zone = parts[0];
                        command = parts[1];
                    }
                    else
                    {
                        zone = defaultZone;
                        command = parts[0];
                    }

                    // Split arguments by comma or space
                    argumentsList = new List<string>(
                        from a in commandArguments.Split(',', ' ')
                        select norm(a)
                    );
                }
                else
                {
                    // Split command part by space or dot
                    var parts = new List<string>(
                        from c in command.Split(commandSep)
                        select norm(c)
                    );

                    if (parts.Count >= 3)
                    {
                        zone = parts[0];
                        command = parts[1];
                        argumentsList = parts.GetRange(2, parts.Count - 2);
                    }
                    else if (parts.Count == 2)
                    {
                        zone = defaultZone;
                        command = parts[0];
                        argumentsList = parts.GetRange(1, 1);
                    }
                    else
                    {
                        throw new ArgumentException("Need at least command and argument");
                    }
                }
            }

            //zone = zone ?? "";

            // Find the command in our database, resolve to internal eISCP command
            object group = EiscpCommands.ZoneMappings[zone] ?? zone;
            if (!EiscpCommands.Commands.Contains(group))
            {
                throw new ArgumentException(String.Format("\"{0}\" is not a valid zone", group));
            }

            object prefix = Nav(EiscpCommands.CommandMappings, group, command) ?? command;
            if (Nav(EiscpCommands.Commands, group, prefix) == null)
            {
                throw new ArgumentException(String.Format("\"{0}\" is not a valid command in zone \"{1}\"", command, zone));
            }

            // TODO: For now, only support one; though some rare commands would
            // need multiple.
            string argument = argumentsList[0];

            object value = Nav(EiscpCommands.ValueMappings, group, prefix, argument) ?? argument;
            if (Nav(EiscpCommands.Commands, group, prefix, "values", value) == null)
            {
                throw new ArgumentException(String.Format("\"{0}\" is not a valid argument " +
                    "for command \"{1}\" in zone \"{2}\"", argument, command, zone));
            }

            return (string)prefix + (string)value;
        }

        public static Tuple<string, string> IscpToCommand(string iscpMessage)
        {
            foreach (DictionaryEntry item in EiscpCommands.Commands)
            {
                string zone = (string)item.Key;
                IDictionary zoneCmds = (IDictionary)item.Value;

                // For now, ISCP commands are always three characters, which
                // makes this easy.
                string command = iscpMessage.Substring(0, 3);
                string args = iscpMessage.Substring(3);
                if (zoneCmds.Contains(command))
                {
                    if (Nav(zoneCmds, command, "values", args) != null)
                    {
                        return new Tuple<string, string>(
                            (string)Nav(zoneCmds, command, "name"),
                            (string)Nav(zoneCmds, command, "values", args, "name")
                        );
                    }
                }
            }

            throw new ArgumentException("Cannot convert ISCP message to command: " + iscpMessage);
        }

        /// <summary>
        /// Helper that calls <paramref name="getterFunc"/> until a matching message
        /// is found, or the timeout occurs. 
        /// </summary>
        /// Matching means the same commands
        /// group, i.e. for sent message MVLUP we would accept MVL13
        /// in response.
        public static byte[] FilterForMessage(Func<double, byte[]> getterFunc, string msg)
        {
            long start = DateTime.Now.Ticks; // 10 000 ticks makes a millisecond

            while (true)
            {
                byte[] candidate = getterFunc(0.05);
                if (candidate != null)
                {
                    string str = Encoding.ASCII.GetString(candidate);
                    // It seems ISCP commands are always three characters.
                    if (str.Substring(0, 3) == msg.Substring(0, 3))
                    {
                        return candidate;
                    }
                }

                // The protocol docs claim that a response  should arrive
                // within *50ms or the communication as failed*. In my tests,
                // however, the interval needed to be at least 200ms before
                // I managed to see any response, aand only after 300ms
                // reproducably, so use a generous timeout.
                if (DateTime.Now.Ticks - start > 7000000) // 700ms
                {
                    throw new ArgumentException("Not received a response");
                }
            }
        }
    }
}
