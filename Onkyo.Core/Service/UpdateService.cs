using Eiscp.Core.Commands;
using Eiscp.Core.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Onkyo.Core.Service
{
    public static class UpdateService
    {
        public static List<IReceiver> Receivers { get; private set; }

        private static readonly List<string> _sendCommandsQueue = new List<string>();
        private static bool _waitforReceiveQuery;

        public static List<BaseCommand> Commands =>
            new List<BaseCommand>()
            {
                new PWRCommand(),
                new MVLCommand(),
                new AMTCommand(),
                new SLICommand()
            };

        public static void Discover()
        {
            Receivers = Helper.Onkyo.Discover();
            Debug.WriteLine("Discovered receivers");
        }

        public static void Get()
        {
            if (!(Receivers != null && Receivers.Count > 0))
                Discover();

            try
            {
                ThreadPool.QueueUserWorkItem(StartCommandQuery);
            }
            catch (Exception ex)
            {
                Get();
            }
        }

        private static async void StartCommandQuery(object state)
        {
            var sendCommands = _sendCommandsQueue.ToList();
            _sendCommandsQueue.Clear();
            //if (!_waitforReceiveQuery)
           // {
                sendCommands.AddRange(Commands.Select(c => c.Query).ToList());
            //}
            _waitforReceiveQuery = false;
            foreach (var sc in sendCommands)
                Debug.WriteLine("SendCommands: " + sc);
            await Task.Delay(100);

            foreach (var receiver in Receivers)
            {
                var responses = Helper.Onkyo.ExecuteCommands(receiver, sendCommands);
                foreach (var response in responses)
                {
                    try
                    {
                        foreach (var command in Commands)
                        {
                            if(!_waitforReceiveQuery)
                                MessagingCenter.Instance.Send(command, "UPDATE_AVR_COMMANDS", response);
                        }
                    }
                    catch
                    {
                        return;
                    }
                }
                //_waitforReceiveQuery = false;
                await Task.Delay(100);
                StartCommandQuery(Receivers);
            }
        }

        public static void Send(string command)
        {
            _waitforReceiveQuery = true;
            var key = command.Substring(0, 3);
            _sendCommandsQueue.RemoveAll(s => s.StartsWith(key));
            _sendCommandsQueue.Add(command);
        }
    }
}
