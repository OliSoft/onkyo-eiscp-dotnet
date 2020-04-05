using System;
using System.Net;

namespace Eiscp.Core.Interface
{
    public interface IReceiver : IDisposable
    {
        void Send(string iscpMessage);
        byte[] Get(double timeout);
        byte[] Raw(string message);
        string Model { get; }
        IPAddress Host { get; }
        int Port { get; }
    }
}
