using System;
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

        public void setIPTextBox(string text)
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

        private void CompressButton_Click(object sender, EventArgs e)
        {
            if (IPTextBox.Text.Length > 0)
            {
                if (Regex.Match(IPTextBox.Text, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}").Success)
                {
                    ConnectionManager.SimpleConnect(IPTextBox.Text, PORT_NO);
                }
                else
                {
                    ConnectionManager.DNSConnect(IPTextBox.Text, PORT_NO);
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
            Invoke((Action)(() => { FileSaveDialog.ShowDialog(); }));
            var myStream = FileSaveDialog.OpenFile();
            myStream.Write(fileBytes, 0, fileBytes.Length);
            myStream.Close();
        }

        public string FileSelectTextBoxText()
        {
            return FileSelectTextBox.Text;
        }
    }
}
