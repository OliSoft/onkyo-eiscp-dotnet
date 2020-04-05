namespace Eiscp.Core.Model
{
    public class Header
    {
        public byte[] magic;
        public int headerSize;
        public int messageSize;
        public byte version;
        public byte[] reserved;
    }
}
