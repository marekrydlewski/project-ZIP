using System;
using System.Deployment.Application;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace project_ZIP.client
{
    public partial class ProjectZip : Form
    { 
        private Form _window;
        private string PORT_NO = "1234";

        private delegate void setIPTextBoxCallback(string text);

        private delegate void setControlsCallback(bool state);
 
        public ProjectZip()
        {
            InitializeComponent();
            _window = this;
        }

        private void setIPTextBox(string text)
        {
            if (IPTextBox.InvokeRequired)
            {
                setIPTextBoxCallback IPTextBoxCallback = setIPTextBox;
                _window.Invoke(IPTextBoxCallback, text);
            }
            else
            {
                IPTextBox.Text = text;
            }
        }

        public void setControls(bool state)
        {
            if (IPTextBox.InvokeRequired || FileSelectButton.InvokeRequired || FileSelectTextBox.InvokeRequired)
            {
                setControlsCallback controlsCallback = setControls;
                _window.Invoke(controlsCallback, state);
            }
            else
            {
                FileSelectTextBox.Enabled = state;
                FileSelectButton.Enabled = state;
                IPTextBox.Enabled = state;
                CompressButton.Enabled = state;
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                /* read data */
                int size = socketFd.EndReceive(ar);

                if (size > 0)
                {
                    state.m_StringBuilder.Append(Encoding.ASCII.GetString(state.m_DataBuf, 0, size));

                    /* get the rest of the data */
                    socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                {
                    /* all the data has arrived */
                    if (state.m_StringBuilder.Length > 1)
                    {

                        /* shutdown and close socket */
                        socketFd.Shutdown(SocketShutdown.Both);
                        socketFd.Close();
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message);
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the socket from the state object */
                Socket socketFd = (Socket)ar.AsyncState;

                /* complete the connection */
                socketFd.EndConnect(ar);

                setControls(false);

                DirectorySender.SendDirectory(FileSelectTextBox.Text, socketFd);

                FileReceiver.FileReceive(socketFd);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message);
            }
        }

        private void GetHostEntryCallback(IAsyncResult ar)
        {
            try
            {
                /* complete the DNS query */
                var hostEntry = Dns.EndGetHostEntry(ar);
                var addresses = hostEntry.AddressList;

                /* create a socket */
                var socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                /* remote endpoint for the socket */
                var endPoint = new IPEndPoint(addresses[0], Int32.Parse(PORT_NO));

                setIPTextBox("");

                /* connect to the server */
                socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), socketFd);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message);
            }
        }

        private void CompressButton_Click(object sender, EventArgs e)
        {
            if (IPTextBox.Text.Length > 0)
            {
                if (Regex.Match(IPTextBox.Text, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}").Success)
                {
                    var socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    var endPoint = new IPEndPoint(IPAddress.Parse(IPTextBox.Text), Int32.Parse(PORT_NO));

                    setIPTextBox("");

                    /* connect to the server */
                    socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), socketFd);
                }
                else
                {
                    Dns.BeginGetHostEntry(IPTextBox.Text, GetHostEntryCallback, null);
                }         
            }
            else
            {
                MessageBox.Show("IP address empty");
            }
        }

        private void FileSelectButton_Click(object sender, EventArgs e)
        {
            if (DirectorySelectDialog.ShowDialog() == DialogResult.OK)
            {
                FileSelectTextBox.Text = DirectorySelectDialog.SelectedPath;
            }
        }

        public void DownloadFile(byte[] fileBytes)
        {
            if (FileSaveDialog.ShowDialog() == DialogResult.OK)
            {
                var myStream = FileSaveDialog.OpenFile();
                myStream.Write(fileBytes, 0, fileBytes.Length);
                myStream.Close();
            }

        }
    }
}
