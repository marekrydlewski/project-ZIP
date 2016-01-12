using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace project_ZIP.client
{
    public class DirectorySender
    {
        public enum  SendDirectoryStatus
        {
            OK, ERROR
        }

        public static SendDirectoryStatus SendDirectory(string path, Collection<string> fileMap = null, string parentDirectory = "")
        {
            bool entryPoint = (fileMap == null ? true : false);

            if (fileMap == null) fileMap = new Collection<string>();

            string dirName = Path.Combine(parentDirectory, Path.GetFileName(path));

            string[] files = Directory.GetFiles(path);
            string[] subdirectories = Directory.GetDirectories(path);

            //send files

            foreach (var fileName in files)
            {
                fileMap.Add(Path.Combine(dirName, Path.GetFileName(fileName)));
            }

            foreach (var subdirectoryPath in subdirectories)
            {
                if (SendDirectory(subdirectoryPath, fileMap, dirName) == SendDirectoryStatus.ERROR)
                {
                    return SendDirectoryStatus.ERROR;
                }
            }

            //if entryPoint send filemap

            return SendDirectoryStatus.OK;
        }
    }
}
