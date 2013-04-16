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
            this.Version = new System.Windows.Forms.TextBox();
            this.UseGNUPlot = new System.Windows.Forms.CheckBox();
            this.LocateGNUPlot = new System.Windows.Forms.Button();
            this.GNUPlotLocation = new System.Windows.Forms.TextBox();
            this.GNUPlotBrowser = new System.Windows.Forms.OpenFileDialog();
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
            // Version
            // 
            this.Version.Location = new System.Drawing.Point(19, 196);
            this.Version.Name = "Version";
            this.Version.ReadOnly = true;
            this.Version.Size = new System.Drawing.Size(772, 20);
            this.Version.TabIndex = 7;
            this.Version.Text = "Version Info";
            this.Version.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // UseGNUPlot
            // 
            this.UseGNUPlot.AutoSize = true;
            this.UseGNUPlot.Checked = true;
            this.UseGNUPlot.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UseGNUPlot.Location = new System.Drawing.Point(19, 163);
            this.UseGNUPlot.Name = "UseGNUPlot";
            this.UseGNUPlot.Size = new System.Drawing.Size(82, 17);
            this.UseGNUPlot.TabIndex = 8;
            this.UseGNUPlot.Text = "use gnuPlot";
            this.UseGNUPlot.UseVisualStyleBackColor = true;
            this.UseGNUPlot.CheckedChanged += new System.EventHandler(this.UseGNUPlot_CheckedChanged);
            // 
            // LocateGNUPlot
            // 
            this.LocateGNUPlot.Location = new System.Drawing.Point(102, 159);
            this.LocateGNUPlot.Name = "LocateGNUPlot";
            this.LocateGNUPlot.Size = new System.Drawing.Size(105, 23);
            this.LocateGNUPlot.TabIndex = 9;
            this.LocateGNUPlot.Text = "Locate GNU Plot";
            this.LocateGNUPlot.UseVisualStyleBackColor = true;
            this.LocateGNUPlot.Click += new System.EventHandler(this.LocateGNUPlot_Click);
            // 
            // GNUPlotLocation
            // 
            this.GNUPlotLocation.Location = new System.Drawing.Point(218, 161);
            this.GNUPlotLocation.Name = "GNUPlotLocation";
            this.GNUPlotLocation.ReadOnly = true;
            this.GNUPlotLocation.Size = new System.Drawing.Size(573, 20);
            this.GNUPlotLocation.TabIndex = 10;
            this.GNUPlotLocation.Text = "gnuPlot Dir";
            this.GNUPlotLocation.TextChanged += new System.EventHandler(this.GNUPlotLocation_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(815, 228);
            this.Controls.Add(this.GNUPlotLocation);
            this.Controls.Add(this.LocateGNUPlot);
            this.Controls.Add(this.UseGNUPlot);
            this.Controls.Add(this.Version);
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
        private System.Windows.Forms.TextBox Version;
        private System.Windows.Forms.CheckBox UseGNUPlot;
        private System.Windows.Forms.Button LocateGNUPlot;
        private System.Windows.Forms.TextBox GNUPlotLocation;
        private System.Windows.Forms.OpenFileDialog GNUPlotBrowser;
    }
}

