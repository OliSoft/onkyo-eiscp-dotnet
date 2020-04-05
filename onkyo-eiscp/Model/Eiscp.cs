using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Eiscp.Core.Interface;
using Eiscp.Core.Helper;
using Eiscp.Core.Message;

namespace Eiscp.Core.Model
{
    /// <summary>
    /// Implements the eISCP interface to Onkyo receivers.
    /// </summary>
    ///
    /// This uses a blocking interface. The remote end will regularily
    /// send unsolicited status updates. You need to manually call
    /// <see cref="Get"/> to query those.
    /// 
    /// You may want to look at the <see cref="Receiver"/> class instead, which
    /// uses a background thread.
    public class Eiscp : IReceiver
    {
        private Socket commandSocket;

        public int Port
        {
            get;
            protected set;
        }

        public IPAddress Host
        {
            get;
            protected set;
        }

        public string Model
        {
            get;
            protected set;
        }

        /// <summary>
        /// Try to find ISCP devices on network.
        /// </summary>
        /// Waits for <paramref name="timeout"/> seconds, then returns all devices found,
        /// in form of a list of <see cref="IReceiver"/>s.        
        public static List<IReceiver> Discover(double timeout, Func<IPAddress, int, string, IReceiver> constructor)
        {
            int onkyoPort = 60128;
            byte[] onkyoMagic = (new EiscpPacket("!xECNQSTN")).Bytes;

            var foundReceivers = new List<IReceiver>();


            // Broadcast magic
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            using(socket)
            {
                socket.Blocking = false; // So we can use Poll
                socket.EnableBroadcast = true;
                //test
                //var ipAddress = IPAddress.Parse("192.168.0.200");
                //test end

                socket.Bind(new IPEndPoint(IPAddress.Any, 0));
                socket.SendTo(onkyoMagic, new IPEndPoint(IPAddress.Broadcast, onkyoPort));
                //socket.SendTo(onkyoMagic, new IPEndPoint(ipAddress, onkyoPort));

                EndPoint addr = new IPEndPoint(IPAddress.Broadcast, onkyoPort);
                byte[] data = new byte[1024];
                int read = 0;
                while (true)
                {
                    int microsecondTimeout = (int)(timeout * 1000000);
                    if (!socket.Poll(microsecondTimeout, SelectMode.SelectRead))
                    {
                        break;
                    }
                    read = socket.ReceiveFrom(data, ref addr);

                    string response = Encoding.ASCII.GetString(EiscpPacket.Parse(data));
                    // Return string looks something like this:
                    // !1ECNTX-NR609/60128/DX
                    GroupCollection info = Regex.Match(response.Trim(),
                        @"!" +
                        @"(?<device_category>\d)" +
                        @"ECN" +
                        @"(?<model_name>[^/]*)/" +
                        @"(?<iscp_port>\d{5})/" +
                        @"(?<area_code>\w{2})/" +
                        @"(?<identifier>.*)"
                    ).Groups;

                    int port = Int32.Parse(info["iscp_port"].Value);
                    string model = info["model_name"].Value;

                    // Give the user a ready-made receiver instance. It will only
                    // connect on demand, when actually used.
                    IReceiver receiver = constructor((addr as IPEndPoint).Address, port, model);
                    foundReceivers.Add(receiver);
                }
            }

            return foundReceivers;
        }

        public static List<IReceiver> Discover(double timeout = 5)
        {
            return Discover(timeout, Create);
        }

        public static Eiscp Create(IPAddress address, int port, string model)
        {
            return new Eiscp(address, port, model);
        }

        public Eiscp(IPAddress host, int port = 60128, string model = null)
        {
            Host = host;
            Port = port;
            Model = model;

            commandSocket = null;
        }

        ~Eiscp()
        {
            Dispose();
        }

        public override string ToString()
        {            
            return String.Format("<{0}({1}) {2}:{3}>", GetType(), Model ?? "unknown", Host, Port);
        }

        protected bool EnsureSocketConnected()
        {
            if (commandSocket == null)
            {
                try
                {
                    commandSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    commandSocket.Connect(Host, Port);
                    commandSocket.Blocking = false;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public virtual void Disconnect()
        {
            try
            {
                commandSocket.Close();
            }
            catch (Exception)
            { }

            commandSocket = null;
        }

        public void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }
       
        /// <summary>
        /// Send a low-level ISCP message, like "MVL50".
        /// </summary>
        ///
        /// This does not return anything, nor does it wait for a response
        /// from the receiver. You can query responses via <see cref="Get"/>,
        /// or use <see cref="Raw"/> to send a message and waiting for one.
        public virtual void Send(string iscpMessage)
        {
            if(EnsureSocketConnected())
                commandSocket.Send(Utils.CommandToPacket(iscpMessage).Bytes);
        }

        /// Return the next message sent by the receiver, or, after
        /// <paramref name="timeout"/> has passed, return <c>null</c>.
        public virtual byte[] Get(double timeout = 0.1)
        {
            if (EnsureSocketConnected())
            {
                int microsecondTimeout = (int)(timeout * 1000000);
                if (commandSocket.Poll(microsecondTimeout, SelectMode.SelectRead))
                {
                    byte[] headerBytes = new byte[16];
                    commandSocket.Receive(headerBytes);

                    Header header = EiscpPacket.ParseHeader(headerBytes);

                    byte[] message = new byte[header.messageSize];
                    commandSocket.Receive(message);

                    return IscpMessage.Parse(message);
                }
            }
            return null;
        }


        /// <summary>
        /// Send a low-level ISCP message, like "MVL50", and wait
        /// for a response.
        /// </summary>
        ///
        /// <para>
        /// While the protocol is designed to acknowledge each message with
        /// a response, there is no fool-proof way to differentiate those
        /// from unsolicited status updates, though we'll do our best to
        /// try. Generally, this won't be an issue, though in theory the
        /// response this function returns to you sending <c>"SLI05"</c> may be
        /// a <c>"SLI06"</c> update from another controller.
        /// </para>
        /// 
        /// <para>
        /// It'd be preferable to design your app in a way where you are
        /// processing all incoming messages the same way, regardless of
        /// their origin.
        /// </para>
        public virtual byte[] Raw(string iscpMessage)
        {
            while (Get(0.0) != null)
            {
                // Clear all incoming messages. If not yet queried,
                // they are lost. This is so that we can find the real
                // response to our sent command later.
                ;
            }

            Send(iscpMessage);
            return Utils.FilterForMessage(this.Get, iscpMessage);
        }

        /// <summary>
        /// Send a high-level command to the receiver, return the
        /// receiver's response formatted has a command.
        /// </summary>
        ///
        /// This is basically a helper that combines <see cref="Raw"/>,
        /// <see cref="Utils.CommandToIscp"/> and <see cref="Utils.IscpToCommand"/>.
        public Tuple<string, string> Command(string command, string arguments = null, string zone = null)
        {
            var iscpMessage = Utils.CommandToIscp(command, arguments, zone);
            var response = Raw(iscpMessage);
            if (response != null)
            {
                string str = Encoding.ASCII.GetString(response);
                return Utils.IscpToCommand(str);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Turn the receiver power on.
        /// </summary>
        public void PowerOn()
        {
            Command("power", "on");
        }

        /// <summary>
        /// Turn the receiver power off.
        /// </summary>
        public void PowerOff()
        {
            Command("power", "off");
        }
    }
}