using System.Collections.Generic;
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

        public static SendDirectoryStatus SendDirectory(string path, out Dictionary<string, string> newFileMap, Dictionary<string, string> fileMap = null, string parentDirectory = "")
        {
            if (fileMap == null) fileMap = new Dictionary<string, string>();

            string dirName = Path.Combine(parentDirectory, Path.GetFileName(path));

            string[] files = Directory.GetFiles(path);
            string[] subdirectories = Directory.GetDirectories(path);

            //send files

            foreach (var fileName in files)
            {
                fileMap.Add(Path.Combine(dirName, Path.GetFileName(fileName)), dirName);
            }

            foreach (var subdirectoryPath in subdirectories)
            {
                if (SendDirectory(subdirectoryPath, out newFileMap, fileMap, dirName) == SendDirectoryStatus.OK)
                {
                    fileMap = newFileMap;
                }
                else
                {
                    newFileMap = null;
                    return SendDirectoryStatus.ERROR;
                }
            }
            newFileMap = fileMap;
            return SendDirectoryStatus.OK;
        }
    }
}
