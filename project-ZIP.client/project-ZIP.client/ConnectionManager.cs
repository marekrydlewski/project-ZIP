using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace project_ZIP.client
{
    public class ConnectionManager
    {
        public static void SimpleConnect(string IPaddress, string PORT_NO)
        {
            var socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var endPoint = new IPEndPoint(IPAddress.Parse(IPaddress), int.Parse(PORT_NO));

            /* connect to the server */
            socketFd.BeginConnect(endPoint, ConnectCallback, socketFd);
        }

        public static void DNSConnect(string hostName, string PORT_NO)
        {
            Dns.BeginGetHostEntry(hostName, GetHostEntryCallback, PORT_NO);
        }

        private static void GetHostEntryCallback(IAsyncResult ar)
        {
            try
            {
                var PORT_NO = (string) ar.AsyncState;

                /* complete the DNS query */
                var hostEntry = Dns.EndGetHostEntry(ar);
                var addresses = hostEntry.AddressList;

                /* create a socket */
                var socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                /* remote endpoint for the socket */
                var endPoint = new IPEndPoint(addresses[0], int.Parse(PORT_NO));

                /* connect to the server */
                socketFd.BeginConnect(endPoint, ConnectCallback, socketFd);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message);
                var window = (ProjectZip) Application.OpenForms[0];
                window.SetControls(true);
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the socket from the state object */
                var socketFd = (Socket)ar.AsyncState;

                /* complete the connection */
                socketFd.EndConnect(ar);

                //handle for threads control
                var sendHandle = new ManualResetEvent(false);

                //send directory, wait for finished sending, then receive compressed file
                var window = (ProjectZip) Application.OpenForms[0];
                DirectorySender.SendDirectory(window.FileSelectTextBoxText(), socketFd, sendHandle);

                sendHandle.WaitOne();

                FileReceiver.FileReceive(socketFd);
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
