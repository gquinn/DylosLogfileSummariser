namespace DylosLogfileSummariser
{
    partial class Form1
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
            this.FolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.WorkingDirectory = new System.Windows.Forms.TextBox();
            this.DirectoryBrowser = new System.Windows.Forms.Button();
            this.Summarise = new System.Windows.Forms.Button();
            this.Status = new System.Windows.Forms.Label();
            this.FileStatus = new System.Windows.Forms.Label();
            this.Step = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // WorkingDirectory
            // 
            this.WorkingDirectory.Location = new System.Drawing.Point(156, 25);
            this.WorkingDirectory.Name = "WorkingDirectory";
            this.WorkingDirectory.ReadOnly = true;
            this.WorkingDirectory.Size = new System.Drawing.Size(635, 20);
            this.WorkingDirectory.TabIndex = 3;
            // 
            // DirectoryBrowser
            // 
            this.DirectoryBrowser.Location = new System.Drawing.Point(19, 22);
            this.DirectoryBrowser.Name = "DirectoryBrowser";
            this.DirectoryBrowser.Size = new System.Drawing.Size(131, 23);
            this.DirectoryBrowser.TabIndex = 1;
            this.DirectoryBrowser.Text = "Choose Logfile Directory";
            this.DirectoryBrowser.UseVisualStyleBackColor = true;
            this.DirectoryBrowser.Click += new System.EventHandler(this.DirectoryBrowser_Click);
            // 
            // Summarise
            // 
            this.Summarise.Location = new System.Drawing.Point(19, 65);
            this.Summarise.Name = "Summarise";
            this.Summarise.Size = new System.Drawing.Size(131, 23);
            this.Summarise.TabIndex = 2;
            this.Summarise.Text = "Summarise Files";
            this.Summarise.UseVisualStyleBackColor = true;
            this.Summarise.Click += new System.EventHandler(this.Summarise_Click);
            // 
            // Status
            // 
            this.Status.AutoSize = true;
            this.Status.Location = new System.Drawing.Point(159, 97);
            this.Status.MaximumSize = new System.Drawing.Size(635, 20);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(48, 13);
            this.Status.TabIndex = 4;
            this.Status.Text = "Progress";
            // 
            // FileStatus
            // 
            this.FileStatus.AutoSize = true;
            this.FileStatus.Location = new System.Drawing.Point(159, 118);
            this.FileStatus.MaximumSize = new System.Drawing.Size(635, 20);
            this.FileStatus.Name = "FileStatus";
            this.FileStatus.Size = new System.Drawing.Size(53, 13);
            this.FileStatus.TabIndex = 5;
            this.FileStatus.Text = "FileStatus";
            // 
            // Step
            // 
            this.Step.AutoSize = true;
            this.Step.Location = new System.Drawing.Point(159, 71);
            this.Step.MaximumSize = new System.Drawing.Size(635, 20);
            this.Step.Name = "Step";
            this.Step.Size = new System.Drawing.Size(29, 13);
            this.Step.TabIndex = 6;
            this.Step.Text = "Step";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 157);
            this.Controls.Add(this.Step);
            this.Controls.Add(this.FileStatus);
            this.Controls.Add(this.Status);
            this.Controls.Add(this.Summarise);
            this.Controls.Add(this.DirectoryBrowser);
            this.Controls.Add(this.WorkingDirectory);
            this.Name = "Form1";
            this.Text = "Dylos Logfile Summariser";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog FolderBrowser;
        private System.Windows.Forms.TextBox WorkingDirectory;
        private System.Windows.Forms.Button DirectoryBrowser;
        private System.Windows.Forms.Button Summarise;
        private System.Windows.Forms.Label Status;
        private System.Windows.Forms.Label FileStatus;
        private System.Windows.Forms.Label Step;
    }
}

