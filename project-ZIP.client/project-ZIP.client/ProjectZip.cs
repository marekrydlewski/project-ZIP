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

        private delegate void setFileSelectTextBoxCallback(string text);
 
        public ProjectZip()
        {
            InitializeComponent();
            _window = this;
        }

        public void SetIPTextBox(string text)
        {
            if (IPTextBox.InvokeRequired)
            {
                setIPTextBoxCallback IPTextBoxCallback = SetIPTextBox;
                _window.Invoke(IPTextBoxCallback, text);
            }
            else
            {
                IPTextBox.Text = text;
            }
        }

        //set file select text box
        public void SetFileSelectTextBox(string text)
        {
            if (FileSelectTextBox.InvokeRequired)
            {
                setFileSelectTextBoxCallback FileSelectTextBoxCallback = SetFileSelectTextBox;
                _window.Invoke(FileSelectTextBoxCallback, text);
            }
            else
            {
                FileSelectTextBox.Text = text;
            }
        }

        //set controls on form to enabled/disabled
        public void SetControls(bool state)
        {
            if (IPTextBox.InvokeRequired || FileSelectButton.InvokeRequired || FileSelectTextBox.InvokeRequired)
            {
                setControlsCallback controlsCallback = SetControls;
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
            if (IPTextBox.Text.Length > 0 && FileSelectTextBox.Text.Length > 0)
            {
                SetControls(false);
                if (Regex.Match(IPTextBox.Text, @"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}").Success)
                {
                    ConnectionManager.SimpleConnect(IPTextBox.Text, PORT_NO);
                }
                else
                {
                    ConnectionManager.DNSConnect(IPTextBox.Text, PORT_NO);
                }         
            }
            else if(IPTextBox.Text.Length == 0)
            {
                MessageBox.Show(@"IP address empty");
            }
            else
            {
                MessageBox.Show(@"No directory selected");
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
