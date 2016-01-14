using System.Net.Sockets;
using System.Threading;

namespace project_ZIP.client
{
    public class FileAndSize
    {
        public int FileSize = 0;
        public int SizeRemaining = 0;
        public byte[] File = new byte[0];
        public byte[] Buffer = new byte[BUF_SIZE];
        public Socket SocketFd = null;
        public const int BUF_SIZE = 1024;
        public ManualResetEvent Handle = null;
    }
}
