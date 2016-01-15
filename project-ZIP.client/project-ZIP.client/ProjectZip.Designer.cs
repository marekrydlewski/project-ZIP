namespace project_ZIP.client
{
    partial class ProjectZip
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.IPTextBox = new System.Windows.Forms.TextBox();
            this.IPLabel = new System.Windows.Forms.Label();
            this.CompressButton = new System.Windows.Forms.Button();
            this.FileSelectButton = new System.Windows.Forms.Button();
            this.FileSelectTextBox = new System.Windows.Forms.TextBox();
            this.FileSelectLabel = new System.Windows.Forms.Label();
            this.DirectorySelectDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.FileSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // IPTextBox
            // 
            this.IPTextBox.Location = new System.Drawing.Point(70, 12);
            this.IPTextBox.Name = "IPTextBox";
            this.IPTextBox.Size = new System.Drawing.Size(202, 20);
            this.IPTextBox.TabIndex = 1;
            // 
            // IPLabel
            // 
            this.IPLabel.AutoSize = true;
            this.IPLabel.Location = new System.Drawing.Point(13, 15);
            this.IPLabel.Name = "IPLabel";
            this.IPLabel.Size = new System.Drawing.Size(51, 13);
            this.IPLabel.TabIndex = 2;
            this.IPLabel.Text = "Server IP";
            // 
            // CompressButton
            // 
            this.CompressButton.Location = new System.Drawing.Point(100, 89);
            this.CompressButton.Name = "CompressButton";
            this.CompressButton.Size = new System.Drawing.Size(75, 23);
            this.CompressButton.TabIndex = 3;
            this.CompressButton.Text = "Compress";
            this.CompressButton.UseVisualStyleBackColor = true;
            this.CompressButton.Click += new System.EventHandler(this.CompressButton_Click);
            // 
            // FileSelectButton
            // 
            this.FileSelectButton.Location = new System.Drawing.Point(197, 63);
            this.FileSelectButton.Name = "FileSelectButton";
            this.FileSelectButton.Size = new System.Drawing.Size(75, 23);
            this.FileSelectButton.TabIndex = 4;
            this.FileSelectButton.Text = "Select...";
            this.FileSelectButton.UseVisualStyleBackColor = true;
            this.FileSelectButton.Click += new System.EventHandler(this.FileSelectButton_Click);
            // 
            // FileSelectTextBox
            // 
            this.FileSelectTextBox.Location = new System.Drawing.Point(16, 63);
            this.FileSelectTextBox.Name = "FileSelectTextBox";
            this.FileSelectTextBox.Size = new System.Drawing.Size(175, 20);
            this.FileSelectTextBox.TabIndex = 5;
            // 
            // FileSelectLabel
            // 
            this.FileSelectLabel.AutoSize = true;
            this.FileSelectLabel.Location = new System.Drawing.Point(13, 47);
            this.FileSelectLabel.Name = "FileSelectLabel";
            this.FileSelectLabel.Size = new System.Drawing.Size(175, 13);
            this.FileSelectLabel.TabIndex = 6;
            this.FileSelectLabel.Text = "Select folder you want to compress:";
            // 
            // FileSaveDialog
            // 
            this.FileSaveDialog.DefaultExt = "zip";
            this.FileSaveDialog.FileName = "compressed";
            this.FileSaveDialog.Filter = "ZIP archive (*.zip)|*.zip";
            // 
            // ProjectZip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 124);
            this.Controls.Add(this.FileSelectLabel);
            this.Controls.Add(this.FileSelectTextBox);
            this.Controls.Add(this.FileSelectButton);
            this.Controls.Add(this.CompressButton);
            this.Controls.Add(this.IPLabel);
            this.Controls.Add(this.IPTextBox);
            this.Name = "ProjectZip";
            this.Text = "Project ZIP";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox IPTextBox;
        private System.Windows.Forms.Label IPLabel;
        private System.Windows.Forms.Button CompressButton;
        private System.Windows.Forms.Button FileSelectButton;
        private System.Windows.Forms.TextBox FileSelectTextBox;
        private System.Windows.Forms.Label FileSelectLabel;
        private System.Windows.Forms.FolderBrowserDialog DirectorySelectDialog;
        private System.Windows.Forms.SaveFileDialog FileSaveDialog;
    }
}

