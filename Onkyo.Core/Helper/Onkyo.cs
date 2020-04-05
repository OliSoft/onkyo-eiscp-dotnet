using Eiscp.Core.Commands;
using Eiscp.Core.Helper;
using Eiscp.Core.Interface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Onkyo.Core.Helper
{
    public static class Onkyo
    {

        private const string usage =
@"Control Onkyo A/V receivers.

Usage:
  onkyo [--host <host>] [--port <port>] [--all] [--name <name>] <command>...
  onkyo --discover
  onkyo --help-commands [<zone> <command>]
  onkyo -h | --help

Selecting the receiver:

  --host, -t <host>     Connect to this host
  --port, -p <port>     Connect to this port [default: 60128]
  --all, -a             Discover receivers, send to all found
  --name, -n <name>     Discover receivers, send to those matching name.

If none of these options is given, the program searches for receivers,
and uses the first one found.

  --discover            List all discoverable receivers
  --help-commands       List available commands.

Examples:
  onkyo power:on source:pc volume:75
    Turn receiver on, select ""PC"" source, set volume to 75.
  onkyo zone2.power:standby
    To execute a command for zone that isn't the main one.";


        //    if (args.Length == 0)
        //    {
        //        Console.WriteLine(usage);
        //        return 1;
        //    }

        //    string baseName = Path.GetFileName(Environment.GetCommandLineArgs()[0]);

        //    bool all = false;
        //    bool discover = false;
        //    bool commandHelp = false;
        //    bool help = false;
        //    string name = null;
        //    string host = null;
        //    string port = null;

        //    var optionSet = new OptionSet()
        //    {
        //        { "t|host=", "Connect to this host", v => { host = v; } },
        //        { "p|port=", "Connect to this port [default: 60128]", v => { port = v; } },
        //        { "a|all", "Discover receivers, send to all found", v => { all = v != null; } },
        //        { "n|name=", "Discover receivers, send to those matching name", v => { name = v; } },
        //        { "discover", "List all discoverable receivers", v => { discover = v != null; } },
        //        { "help-commands", "List available commands", v => { commandHelp = v != null; } },
        //        { "h|help", "Print help", v => { help = v != null; } }
        //    };

        //    List<string> command = new List<string>();

        //    try
        //    {
        //        command = optionSet.Parse(args);
        //    }
        //    catch (OptionException e)
        //    {
        //        Console.WriteLine(e.Message);
        //        Console.WriteLine(usage);

        //        return 1;
        //    }

        //    if (help)
        //    {
        //        Console.WriteLine(usage);
        //        return 0;
        //    }

        //}

        public static List<IReceiver> Discover()
        {
            var receivers = new List<IReceiver>();
            foreach (var receiver in Eiscp.Core.Model.Eiscp.Discover(timeout: 1))
            {
                Console.WriteLine("{0} {1}:{2}", receiver.Model, receiver.Host, receiver.Port);
                receivers.Add(receiver);
            }

            return receivers;
        }

        public static int CommandHelp(List<string> command)
        {
            // List available commands

            // List the zones
            if (command.Count == 0) // no zone, no command
            {
                Console.WriteLine("Available zones:");
                foreach (string zone in EiscpCommands.Commands.Keys)
                {
                    Console.WriteLine(zone);
                }

                return 1;
            }


            // List the commands
            string selectedZone = command[0];

            if (EiscpCommands.Commands[selectedZone] == null)
            {
                Console.WriteLine("No such zone: " + selectedZone);
                return 1;
            }

            if (command.Count == 1) // zone specified, command not
            {
                Console.WriteLine("Available commands for this zone:");

                IDictionary commandsInZone = (IDictionary)EiscpCommands.Commands[selectedZone];
                foreach (IDictionary commandInfo in commandsInZone.Values)
                {
                    Console.WriteLine("  {0} - {1}", commandInfo["name"], commandInfo["description"]);
                }

                //Console.WriteLine("Use {0} --help-commands {1} <command> to see a list " +
                //    "of possible values.", baseName, selectedZone);

                return 0;
            }

            // List values
            object selectedCommand = Utils.Nav(EiscpCommands.CommandMappings, selectedZone, command[1]) ?? command[1];
            if (Utils.Nav(EiscpCommands.Commands, selectedZone, selectedCommand) == null)
            {
                Console.WriteLine("No such command in the selected zone: " + selectedCommand);
                return 1;
            }

            Console.WriteLine("Possible values for this command:");
            IDictionary valuesForCommandDict = (IDictionary)Utils.Nav(EiscpCommands.Commands, selectedZone, selectedCommand, "values");
            foreach (IDictionary valueInfo in valuesForCommandDict.Values)
            {
                Console.WriteLine("  {0} - {1}", valueInfo["name"], valueInfo["description"]);
            }

            return 0;
        }

        public static IReceiver GetReceiver(string host, string port = null)
        {
            // Determine the receivers the command should run on

            IPAddress[] addresses = new IPAddress[0];
            try
            {
                addresses = Dns.GetHostAddresses(host);
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }

            if (addresses.Length == 0)
            {
                Console.WriteLine("Could not resolve the host");
                return null;
            }

            port = port ?? "60128";
            if (!ushort.TryParse(port, out ushort portNum))
            {
                Console.WriteLine(usage);
                return null;
            }

            return new Eiscp.Core.Model.Eiscp(addresses[0], portNum);
        }

        public static IReceiver GetReceiver(bool all, string name = null)
        {
            var receivers = Eiscp.Core.Model.Eiscp.Discover(timeout: 100);

            if (!all)
            {
                if (name != null)
                {
                    receivers = receivers.Where(r => r.Model.Contains(name)).ToList();
                }
                else
                {
                    receivers = receivers.Take(1).ToList();
                }
            }

            if (receivers.Count == 0)
            {
                Console.WriteLine("No receivers found");
                return null;
            }

            return receivers.FirstOrDefault();
        }

        public static List<string> ExecuteCommands(IReceiver receiver, List<string> command)
        {
            using (receiver)
            {
                var responses = new List<string>();

                foreach (var cmd in command)
                {
                    string iscpCommand = null;
                    bool rawResponse = false;

                    if (cmd.All(ch => char.IsUpper(ch) || char.IsDigit(ch)))
                    {
                        iscpCommand = cmd;
                        rawResponse = true;
                    }
                    else
                    {
                        try
                        {
                            iscpCommand = Utils.CommandToIscp(cmd);
                        }
                        catch (ArgumentException e)
                        {
                            Console.WriteLine("Error: " + e.Message);
                            return null;
                        }
                        rawResponse = false;
                    }

                    Console.WriteLine("{0}: {1}", receiver, iscpCommand);
                    string response;
                    try
                    {
                        response = Encoding.ASCII.GetString(receiver.Raw(iscpCommand));
                        responses.Add(response);
                    }
                    catch (ArgumentException e)
                    {
                        Console.WriteLine(e.Message);
                        return responses;
                    }

                    if (rawResponse)
                    {
                        Console.WriteLine(response);
                    }
                    else
                    {
                        try
                        {
                            Console.WriteLine(Utils.IscpToCommand(response));
                        }
                        catch (ArgumentException e)
                        {
                            Console.WriteLine(e.Message);
                            //return null;
                        }
                    }
                }
                return responses;
            }
        }
    }
}
