using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace project_ZIP.client
{
    public class FileSender
    {
        public enum SendFileStatus
        {
            OK, ERROR
        }

        public static void SendFile(string path, Socket socketFd)
        {
            //send file name
            byte[] pathBytes = Encoding.ASCII.GetBytes(Path.GetFileName(path));
            socketFd.Send(pathBytes, pathBytes.Length, 0);

            //send file size
            byte[] file = System.IO.File.ReadAllBytes(path);
            byte[] fileSizeBytes = BitConverter.GetBytes(file.Length);
            if(BitConverter.IsLittleEndian) Array.Reverse(fileSizeBytes);
            socketFd.Send(fileSizeBytes, fileSizeBytes.Length, 0);

            //send file
            FileAndSize fas = new FileAndSize();
            fas.socketFd = socketFd;
            fas.file = file;
            fas.fileSize = file.Length;
            fas.sizeRemaining = file.Length;

            socketFd.BeginSend(file, 0, FileAndSize.BUF_SIZE, 0, new AsyncCallback(SendFileCallback), fas);
        }

        private static void SendFileCallback(IAsyncResult ar)
        {
            FileAndSize fileAndSize = (FileAndSize) ar.AsyncState;

            Socket socketFd = fileAndSize.socketFd;
            int bytesSent = socketFd.EndSend(ar);

            fileAndSize.sizeRemaining -= bytesSent;

            if (fileAndSize.sizeRemaining > 0)
            {
                socketFd.BeginSend(fileAndSize.file, (fileAndSize.fileSize - fileAndSize.sizeRemaining),
                    FileAndSize.BUF_SIZE, 0, new AsyncCallback(SendFileCallback), fileAndSize);
            }
        }
    }
}
