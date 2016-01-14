using System.IO;
using System.Linq;
using System.Net.Sockets;
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
            string dirName = Path.Combine(parentDirectory, Path.GetFileName(path));

            string[] files = Directory.GetFiles(path);
            string[] subdirectories = Directory.GetDirectories(path);

            foreach (var filePath in files)
            { 
                ManualResetEvent fileHandle = new ManualResetEvent(false);
                FileSender.SendFile(filePath, socketFd, fileHandle);
                fileHandle.WaitOne();
            }

            foreach (var subdirectory in subdirectories)
            {
                ManualResetEvent myHandle = new ManualResetEvent(false);
                var status = SendDirectory(subdirectory, socketFd, myHandle, dirName);
                myHandle.WaitOne();
                if (status == SendDirectoryStatus.Error) return status;
            }

            handle.Set();
            return SendDirectoryStatus.Ok;
        }
    }
}
