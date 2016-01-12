using System;
using System.Collections.Generic;
using System.Linq;
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

        public static SendFileStatus SendFile(string path)
        {
            return SendFileStatus.OK;
        }
    }
}
