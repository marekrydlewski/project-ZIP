using System;
using System.Net.Sockets;

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
            //receive File size
            int size = 0;
            //receive File
            FileAndSize fas = new FileAndSize
            {
                SizeRemaining = size,
                SocketFd = socketFd
            };

            socketFd.BeginReceive(fas.Buffer, 0, FileAndSize.BUF_SIZE, 0,  new AsyncCallback(FileReceiveCallback), fas);

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
            Socket socketFd = fileAndSize.SocketFd;

            int bytesReceived = fileAndSize.SocketFd.EndReceive(ar);

            fileAndSize.SizeRemaining -= bytesReceived;

            fileAndSize.File = Combine(fileAndSize.File, fileAndSize.Buffer);

            if (fileAndSize.SizeRemaining > 0)
            {
                socketFd.BeginReceive(fileAndSize.Buffer, 0, FileAndSize.BUF_SIZE, 0,
                    new AsyncCallback(FileReceiveCallback), fileAndSize);
            }
            else
            {
                //send File back to form and save

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
