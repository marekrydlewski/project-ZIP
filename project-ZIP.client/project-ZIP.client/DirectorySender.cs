using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;

namespace project_ZIP.client
{
    public class DirectorySender
    {
        public enum  SendDirectoryStatus
        {
            OK, ERROR
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

            foreach (var subdirectoryPath in subdirectories)
            {
                if (SendDirectory(subdirectoryPath, socketFd, dirName) == SendDirectoryStatus.ERROR)
                {
                    return SendDirectoryStatus.ERROR;
                }
            }

            return SendDirectoryStatus.OK;
        }
    }
}
