using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace project_ZIP.client
{
    public class DirectorySender
    {
        public enum  SendDirectoryStatus
        {
            Ok, Error
        }

        public static SendDirectoryStatus SendDirectory(string path, Socket socketFd, string parentDirectory = "")
        {
            string dirName = Path.Combine(parentDirectory, Path.GetFileName(path));

            string[] files = Directory.GetFiles(path);
            string[] subdirectories = Directory.GetDirectories(path);

            foreach (var filePath in files)
            { 
                FileSender.SendFile(filePath, socketFd);
            }

            if (subdirectories.Any(subdirectoryPath => SendDirectory(subdirectoryPath, socketFd, dirName) == SendDirectoryStatus.Error))
            {
                return SendDirectoryStatus.Error;
            }

            return SendDirectoryStatus.Ok;
        }
    }
}
