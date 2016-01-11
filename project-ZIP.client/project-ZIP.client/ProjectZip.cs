using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace project_ZIP.client
{
    public partial class ProjectZip : Form
    { 
        private Form window;
        private string PORT_NO = "1234";

        private delegate void setLabelCallback(string text);

        private delegate void setIPTextBoxCallback(string text);
 
        public ProjectZip()
        {
            InitializeComponent();
            window = this;
        }

        private void setIPTextBox(string text)
        {
            if (IPTextBox.InvokeRequired)
            {
                setIPTextBoxCallback IPTextBoxCallback = setIPTextBox;
                window.Invoke(IPTextBoxCallback, text);
            }
            else
            {
                IPTextBox.Text = text;
            }
        }

        private void setLabel(string text)
        {
            if (label1.InvokeRequired)
            {
                setLabelCallback labelCallback = setLabel;
                window.Invoke(labelCallback, text);
            }
            else
            {
                label1.Text = text;
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
                        setLabel(state.m_StringBuilder.ToString());

                        /* shutdown and close socket */
                        socketFd.Shutdown(SocketShutdown.Both);
                        socketFd.Close();
                    }
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                setLabel("OOOOOOOOPS");
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

                /* create the SocketStateObject */
                SocketStateObject state = new SocketStateObject();
                state.m_SocketFd = socketFd;

                setLabel("Yay, connected!");

                /* begin receiving the data */
                socketFd.BeginReceive(state.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                setLabel("OOOOOOOOPS!");
            }
        }

        private void GetHostEntryCallback(IAsyncResult ar)
        {
            try
            {
                IPHostEntry hostEntry = null;
                IPAddress[] addresses = null;
                Socket socketFd = null;
                IPEndPoint endPoint = null;

                /* complete the DNS query */
                hostEntry = Dns.EndGetHostEntry(ar);
                addresses = hostEntry.AddressList;

                /* create a socket */
                socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                /* remote endpoint for the socket */
                endPoint = new IPEndPoint(addresses[0], Int32.Parse(PORT_NO));

                setLabel("Wait! Connecting...");
                setIPTextBox("");

                /* connect to the server */
                socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), socketFd);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception:\t\n" + exc.Message.ToString());
                setLabel("Check \"Server Info\" and try again!");
            }
        }

        private void CompressButton_Click(object sender, EventArgs e)
        {
            if (IPTextBox.Text.Length > 0)
            {
                IPAddress[] addresses = null;
                Socket socketFd = null;
                IPEndPoint endPoint = null;

                //Dns.BeginGetHostEntry(IPTextBox.Text.ToString(), GetHostEntryCallback, null);

                socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                endPoint = new IPEndPoint(IPAddress.Parse(IPTextBox.Text.ToString()), Int32.Parse(PORT_NO));

                setLabel("Wait! Connecting...");
                setIPTextBox("");

                /* connect to the server */
                socketFd.BeginConnect(endPoint, new AsyncCallback(ConnectCallback), socketFd);
            }
            else
            {
                setLabel("IP address empty");
            }
        }

        private void FileSelectButton_Click(object sender, EventArgs e)
        {
            if (FileSelectDialog.ShowDialog() == DialogResult.OK)
            {
                FileSelectTextBox.Text = FileSelectDialog.SelectedPath;
            }
        }
    }
}
