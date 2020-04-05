using Eiscp.Core.Message;
using System;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Eiscp.Core.Model
{
    /// <summary>
    /// For communicating over Ethernet, traditional ISCP messages are
    /// wrapped inside an eISCP package.
    /// </summary>
    public class EiscpPacket
    {
        public EiscpPacket(byte[] iscpMessage)
        {
            // Header format is:
            // - magic: 4 bytes of ASCII characters 'ISCP';
            // - header size: unsigned integer 16;
            // - message size: unsigned integer;
            // - version: byte 1;
            // - reserved: 3 bytes 0.
            // Integers are 32 bit big-endian. There's no padding.
            byte[] magic = Encoding.ASCII.GetBytes("ISCP");
            byte[] headerSize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(16));
            byte[] messageSize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(iscpMessage.Length));
            byte[] versionAndReserved = { 1, 0, 0, 0 };

            Bytes = new byte[16 + iscpMessage.Length];

            magic.CopyTo(Bytes, 0);
            headerSize.CopyTo(Bytes, 4);
            messageSize.CopyTo(Bytes, 8);
            versionAndReserved.CopyTo(Bytes, 12);

            iscpMessage.CopyTo(Bytes, 16);
        }

        public EiscpPacket(string message) :
            this(Encoding.ASCII.GetBytes(message))
        {
        }

        public EiscpPacket(IscpMessage iscpMessage) :
            this(iscpMessage.Bytes)
        {
        }

        public byte[] Bytes
        {
            get;
            private set;
        }

        /// <summary>
        /// Parse the eISCP package given by <paramref name="bytes"/>
        /// </summary>       
        public static byte[] Parse(byte[] bytes)
        {
            byte[] headerBytes = new byte[16];
            Array.Copy(bytes, headerBytes, 16);

            Header h = ParseHeader(headerBytes);

            byte[] data = new byte[h.messageSize];
            Array.Copy(bytes, h.headerSize, data, 0, h.messageSize);

            Debug.Assert(data.Length == h.messageSize);

            return data;
        }

        /// <summary>
        /// Parse the header of an eISCP package.
        /// </summary>
        /// This is useful when reading data in a streaming fashion,
        /// because you can subsequently know the number of bytes to
        /// expect in the packet.        
        public static Header ParseHeader(byte[] bytes)
        {
            // A header is always 16 bytes in length
            Debug.Assert(bytes.Length == 16);

            // Parse the header. Its format is described in the constructor's body.
            var header = new Header()
            {
                magic = new byte[4],
                headerSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 4)),
                messageSize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(bytes, 8)),
                version = bytes[12],
                reserved = new byte[3]
            };
            Array.Copy(bytes, 0, header.magic, 0, 4);
            Array.Copy(bytes, 13, header.reserved, 0, 3);

            // Strangely, the header contains a header size field.
            Debug.Assert(Encoding.ASCII.GetString(header.magic) == "ISCP");
            Debug.Assert(header.headerSize == 16);

            return header;
        }
    }
}
