using Eiscp.Core.Helper;
using Eiscp.Core.Interface;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Eiscp.Core
{
    /// <summary>
    /// Changes the behaviour of <see cref="Eiscp"/> to use a background
    /// thread for network operations.
    /// </summary>
    /// 
    /// This allows receiving messages
    /// from the receiver via a callback:
    /// <example>
    /// <code>
    /// <![CDATA[
    ///    public void OnMessage(IscpMessage message)
    ///    {
    ///        Console.Write(message);
    ///    }
    ///
    ///    var receiver = new Receiver(host, port);
    ///    receiver.Message += new Action<IscpMessage>(OnMessage);
    /// ]]>
    /// </code>
    /// </example>
    /// The argument <c>message</c> is
    public class Receiver : Model.Eiscp
    {
        private bool stop;
        private BlockingCollection<Tuple<string, EventWaitHandle, List<object>>> queue;
        private Thread thread;

        public event Action<byte[]> Message;

        public Receiver(IPAddress host, int port = 60128, string model = "") :
            base(host, port, model)
        {
        }

        ~Receiver()
        {
            Dispose();
        }

        public static Receiver Create(IPAddress address, int port, string model)
        {
            return new Receiver(address, port, model);
        }

        public static new List<IReceiver> Discover(double timeout = 5)
        {
            return Discover(timeout, Receiver.Create);
        }

        private void EnsureThreadRunning()
        {
            if (thread == null)
            {
                stop = false;
                queue = new BlockingCollection<Tuple<string, EventWaitHandle, List<object>>>(
                    new ConcurrentQueue<Tuple<string, EventWaitHandle, List<object>>>()
                );
                thread = new Thread(this.ThreadLoop);
                thread.Start();
            }
        }

        public override void Disconnect()
        {
            stop = true;
            thread.Join();
            thread = null;
        }

        /// <summary>
        /// Like <see cref="Eiscp.Send"/>, but sends asynchronously via the
        /// background thread.
        /// </summary>        
        public override void Send(string iscpMessage)
        {
            EnsureThreadRunning();
            queue.Add(new Tuple<string, EventWaitHandle, List<object>>(iscpMessage, null, null));
        }

        /// <summary>
        /// Not supported by this class. Use the <see cref="Message"/>
        /// event to handle incoming messages.
        /// </summary>       
        public override byte[] Get(double timeout)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Like <see cref="Eiscp.Raw"/>.
        /// </summary>        
        public override byte[] Raw(string iscpMessage)
        {
            EnsureThreadRunning();
            var ev = new EventWaitHandle(false, EventResetMode.ManualReset);
            var result = new List<object>();

            queue.Add(new Tuple<string, EventWaitHandle, List<object>>(iscpMessage, ev, result));
            ev.WaitOne();

            if (result[0] is Exception)
            {
                throw (Exception)result[0];
            }

            return (byte[])result[0];
        }

        private void ThreadLoop()
        {
            if (EnsureSocketConnected())
            {
                try
                {
                    while (!stop)
                    {
                        // Clear all incoming messages first
                        while (true)
                        {
                            byte[] msg = base.Get();
                            if (msg == null)
                                break;
                            Message(msg);
                        }

                        // Send next message                    
                        if (!queue.TryTake(out Tuple<string, EventWaitHandle, List<object>> item, 10)) // 10 ms timeout
                        {
                            continue;
                        }

                        if (item != null)
                        {
                            string message = item.Item1;
                            EventWaitHandle ev = item.Item2;
                            List<object> result = item.Item3;

                            base.Send(message);

                            // Wait for a response, if the caller so desires
                            if (ev != null)
                            {
                                byte[] response = null;
                                try
                                {
                                    // XXX We are losing messages here, since
                                    // those are not triggering the callback!
                                    // Eiscp.Raw() really has the same problem,
                                    // messages being dropped without a chance
                                    // to Get() them. Maybe use a queue after all.
                                    response = Utils.FilterForMessage(base.Get, message);
                                }
                                catch (ArgumentException e)
                                {
                                    // No response within timeout
                                    result.Add(e);
                                }

                                if (response != null)
                                {
                                    result.Add(response);
                                }

                                // Mark as processed
                                ev.Set();
                            }
                        }
                    }
                }
                finally
                {
                    base.Disconnect();
                }
            }            
        }
    }
}
