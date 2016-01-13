using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace project_ZIP.client
{
    public class FileAndSize
    {
        public int fileSize = 0;
        public int sizeRemaining = 0;
        public byte[] file = new byte[0];
        public byte[] buffer = new byte[BUF_SIZE];
        public Socket socketFd = null;
        public const int BUF_SIZE = 1024;
    }
}
