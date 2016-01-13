using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace project_ZIP.client
{
    public class FileSender
    {
        public enum SendFileStatus
        {
            Ok, Error
        }

        public static void SendFile(string path, Socket socketFd)
        {
            //send File name
            byte[] pathBytes = Encoding.ASCII.GetBytes(Path.GetFileName(path));

            byte[] pathSizeBytes = BitConverter.GetBytes(pathBytes.Length);
            //if(BitConverter.IsLittleEndian) Array.Reverse(pathSizeBytes);

            socketFd.Send(pathSizeBytes, pathSizeBytes.Length, 0);

            socketFd.Send(pathBytes, pathBytes.Length, 0);

            //send File size
            byte[] file = File.ReadAllBytes(path);

            byte[] fileSizeBytes = BitConverter.GetBytes(file.Length);
            //if(BitConverter.IsLittleEndian) Array.Reverse(fileSizeBytes);

            socketFd.Send(fileSizeBytes, fileSizeBytes.Length, 0);

            //send File
            FileAndSize fas = new FileAndSize
            {
                SocketFd = socketFd,
                File = file,
                FileSize = file.Length,
                SizeRemaining = file.Length
            };

            socketFd.BeginSend(file, 0, FileAndSize.BUF_SIZE, 0, new AsyncCallback(SendFileCallback), fas);
        }

        private static void SendFileCallback(IAsyncResult ar)
        {
            FileAndSize fileAndSize = (FileAndSize) ar.AsyncState;

            Socket socketFd = fileAndSize.SocketFd;
            int bytesSent = socketFd.EndSend(ar);

            fileAndSize.SizeRemaining -= bytesSent;

            if (fileAndSize.SizeRemaining > 0)
            {
                socketFd.BeginSend(fileAndSize.File, (fileAndSize.FileSize - fileAndSize.SizeRemaining),
                    (fileAndSize.SizeRemaining > FileAndSize.BUF_SIZE ? FileAndSize.BUF_SIZE : fileAndSize.SizeRemaining), 0, new AsyncCallback(SendFileCallback), fileAndSize);
            }
        }
    }
}
