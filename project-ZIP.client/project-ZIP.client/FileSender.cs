using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

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
            var filePath = Path.Combine(parentDirectory, Path.GetFileName(path));
            filePath = filePath.Replace('\\', '/');
            var pathBytes = Encoding.ASCII.GetBytes(filePath);

            var pathSizeBytes = BitConverter.GetBytes(pathBytes.Length);

            try
            {
                socketFd.Send(pathSizeBytes, pathSizeBytes.Length, 0);

                socketFd.Send(pathBytes, pathBytes.Length, 0);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message);
                var window = (ProjectZip)Application.OpenForms[0];
                window.SetControls(true);
            }

            //send File size
            var file = File.ReadAllBytes(path);

            var fileSizeBytes = BitConverter.GetBytes(file.Length);

            try
            {
                socketFd.Send(fileSizeBytes, fileSizeBytes.Length, 0);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message);
                var window = (ProjectZip)Application.OpenForms[0];
                window.SetControls(true);
            }

            //send File
            var fas = new FileAndSize
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
            try
            {
                var fileAndSize = (FileAndSize)ar.AsyncState;

                var socketFd = fileAndSize.SocketFd;

                var handle = fileAndSize.Handle;

                var bytesSent = socketFd.EndSend(ar);

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
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message);
                var window = (ProjectZip)Application.OpenForms[0];
                window.SetControls(true);
            }
        }
    }
}
