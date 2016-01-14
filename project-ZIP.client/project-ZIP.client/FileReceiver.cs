using System;
using System.Net.Sockets;
using System.Windows.Forms;

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
            byte[] sizeBytes = new byte[sizeof(int)];
            socketFd.Receive(sizeBytes, sizeBytes.Length, 0);
            int size = BitConverter.ToInt32(sizeBytes, 0);

            //receive File
            FileAndSize fas = new FileAndSize
            {
                SizeRemaining = size,
                SocketFd = socketFd
            };

            socketFd.BeginReceive(fas.Buffer, 0, FileAndSize.BUF_SIZE, 0,  new AsyncCallback(FileReceiveCallback), fas);

            return FileReceiveStatus.OK;
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
                ProjectZip window = (ProjectZip) Application.OpenForms[0];
                window.setControls(true);
                window.setIPTextBox("");

                //send file back to form
                window.DownloadFile(fileAndSize.File);

                socketFd.Shutdown(SocketShutdown.Both);
                socketFd.Close();
            }
        }

        //method for combining two byte arrays
        private static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }
    }
}
