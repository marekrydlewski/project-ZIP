using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace project_ZIP.client
{
    public class FileSender
    {
        public enum SendFileStatus
        {
            Ok, Error
        }

        public static void SendFile(string path, string parentDirectory, Socket socketFd, ManualResetEvent handle)
        {
            //send File name
            string filePath = Path.Combine(parentDirectory, Path.GetFileName(path));
            filePath = filePath.Replace('\\', '/');
            byte[] pathBytes = Encoding.ASCII.GetBytes(filePath);

            byte[] pathSizeBytes = BitConverter.GetBytes(pathBytes.Length);

            socketFd.Send(pathSizeBytes, pathSizeBytes.Length, 0);

            socketFd.Send(pathBytes, pathBytes.Length, 0);

            //send File size
            byte[] file = File.ReadAllBytes(path);

            byte[] fileSizeBytes = BitConverter.GetBytes(file.Length);

            socketFd.Send(fileSizeBytes, fileSizeBytes.Length, 0);

            //send File
            FileAndSize fas = new FileAndSize
            {
                SocketFd = socketFd,
                File = file,
                FileSize = file.Length,
                SizeRemaining = file.Length,
                Handle = handle
            };

            socketFd.BeginSend(file, 0, (fas.SizeRemaining < FileAndSize.BUF_SIZE ? fas.SizeRemaining : FileAndSize.BUF_SIZE), 0, SendFileCallback, fas);
        }

        private static void SendFileCallback(IAsyncResult ar)
        {
            FileAndSize fileAndSize = (FileAndSize) ar.AsyncState;

            Socket socketFd = fileAndSize.SocketFd;

            ManualResetEvent handle = fileAndSize.Handle;

            int bytesSent = socketFd.EndSend(ar);

            fileAndSize.SizeRemaining -= bytesSent;

            if (fileAndSize.SizeRemaining > 0)
            {
                socketFd.BeginSend(fileAndSize.File, (fileAndSize.FileSize - fileAndSize.SizeRemaining),
                    (fileAndSize.SizeRemaining > FileAndSize.BUF_SIZE ? FileAndSize.BUF_SIZE : fileAndSize.SizeRemaining), 0, SendFileCallback, fileAndSize);
            }
            else
            {
                handle.Set();
            }
        }
    }
}
