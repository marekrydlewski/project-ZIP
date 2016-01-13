using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace project_ZIP.client
{
    public class FileReceiver
    {

        public enum FileReceiveStatus
        {
            OK, ERROR
        }

        public static FileReceiveStatus FileReceive(Socket socketFd)
        {
            //receive file size
            int size = 0;
            //receive file
            FileAndSize fas = new FileAndSize();
            fas.sizeRemaining = size;
            fas.socketFd = socketFd;

            socketFd.BeginReceive(fas.buffer, 0, FileAndSize.BUF_SIZE, 0,  new AsyncCallback(FileReceiveCallback), fas);

            return FileReceiveStatus.OK;
        }

        private int getSize()
        {
            return 0;
        }

        private void SizeReceiveCallback(IAsyncResult ar)
        {
            byte[] size = (byte[]) ar.AsyncState;


        }

        private static void FileReceiveCallback(IAsyncResult ar)
        {
            FileAndSize fileAndSize = (FileAndSize) ar.AsyncState;
            Socket socketFd = fileAndSize.socketFd;

            int bytesReceived = fileAndSize.socketFd.EndReceive(ar);

            fileAndSize.sizeRemaining -= bytesReceived;

            fileAndSize.file = Combine(fileAndSize.file, fileAndSize.buffer);

            if (fileAndSize.sizeRemaining > 0)
            {
                socketFd.BeginReceive(fileAndSize.buffer, 0, FileAndSize.BUF_SIZE, 0,
                    new AsyncCallback(FileReceiveCallback), fileAndSize);
            }
            else
            {
                //send file back to form and save

                socketFd.Shutdown(SocketShutdown.Both);
                socketFd.Close();
            }
        }

        private static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }
    }
}
