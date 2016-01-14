using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace project_ZIP.client
{
    public class DirectorySender
    {
        public enum  SendDirectoryStatus
        {
            Ok, Error
        }

        public static SendDirectoryStatus SendDirectory(string path, Socket socketFd, ManualResetEvent handle, string parentDirectory = "")
        {
            //if it is entry point, send number of files to send
            if(parentDirectory == "") sendFilesNumber(socketFd, Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length);

            //combine parent directory path and current directory path to get full path
            var dirName = Path.Combine(parentDirectory, Path.GetFileName(path));

            //get all files and subdirectories in current directory
            var files = Directory.GetFiles(path);
            var subdirectories = Directory.GetDirectories(path);

            foreach (var filePath in files)
            { 
                var fileHandle = new ManualResetEvent(false);

                FileSender.SendFile(filePath, dirName, socketFd, fileHandle);

                fileHandle.WaitOne();
            }

            foreach (var subdirectory in subdirectories)
            {
                var subdirectoryHandle = new ManualResetEvent(false);

                var status = SendDirectory(subdirectory, socketFd, subdirectoryHandle, dirName);

                subdirectoryHandle.WaitOne();

                if (status == SendDirectoryStatus.Error) return status;
            }

            handle.Set();
            return SendDirectoryStatus.Ok;
        }

        private static void sendFilesNumber(Socket socketFd, int filesNumber)
        {
            var filesNumberBytes = Encoding.ASCII.GetBytes(filesNumber.ToString());
            var filesNumberSizeBytes = BitConverter.GetBytes(filesNumberBytes.Length);

            socketFd.Send(filesNumberSizeBytes, filesNumberSizeBytes.Length, 0);
            socketFd.Send(filesNumberBytes, filesNumberBytes.Length, 0);
        }
    }
}
