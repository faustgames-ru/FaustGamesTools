namespace TexturesMipMapsGenerator
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.label1 = new System.Windows.Forms.Label();
            this._pngSource = new System.Windows.Forms.TextBox();
            this._browsePngSource = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this._generate = new System.Windows.Forms.Button();
            this._browseEtc1Tool = new System.Windows.Forms.Button();
            this._etc1Tool = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._browseTargetFolder = new System.Windows.Forms.Button();
            this._targetFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this._bowsePngSourceDialog = new Ookii.Dialogs.VistaFolderBrowserDialog();
            this._bowseTargetFolderDialog = new Ookii.Dialogs.VistaFolderBrowserDialog();
            this._openEtc1ToolDialog = new System.Windows.Forms.OpenFileDialog();
            this.label4 = new System.Windows.Forms.Label();
            this._status = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "*.png source folder: ";
            // 
            // _pngSource
            // 
            this._pngSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._pngSource.Location = new System.Drawing.Point(146, 12);
            this._pngSource.Name = "_pngSource";
            this._pngSource.Size = new System.Drawing.Size(604, 20);
            this._pngSource.TabIndex = 1;
            // 
            // _browsePngSource
            // 
            this._browsePngSource.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._browsePngSource.Location = new System.Drawing.Point(766, 10);
            this._browsePngSource.Name = "_browsePngSource";
            this._browsePngSource.Size = new System.Drawing.Size(75, 23);
            this._browsePngSource.TabIndex = 2;
            this._browsePngSource.Text = "Browse";
            this._browsePngSource.UseVisualStyleBackColor = true;
            this._browsePngSource.Click += new System.EventHandler(this._browsePngSource_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this._generate);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 234);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(852, 37);
            this.panel1.TabIndex = 3;
            // 
            // _generate
            // 
            this._generate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._generate.Location = new System.Drawing.Point(764, 7);
            this._generate.Name = "_generate";
            this._generate.Size = new System.Drawing.Size(75, 23);
            this._generate.TabIndex = 1;
            this._generate.Text = "Generate";
            this._generate.UseVisualStyleBackColor = true;
            this._generate.Click += new System.EventHandler(this._generate_Click);
            // 
            // _browseEtc1Tool
            // 
            this._browseEtc1Tool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._browseEtc1Tool.Location = new System.Drawing.Point(766, 45);
            this._browseEtc1Tool.Name = "_browseEtc1Tool";
            this._browseEtc1Tool.Size = new System.Drawing.Size(75, 23);
            this._browseEtc1Tool.TabIndex = 6;
            this._browseEtc1Tool.Text = "Browse";
            this._browseEtc1Tool.UseVisualStyleBackColor = true;
            this._browseEtc1Tool.Click += new System.EventHandler(this._browseEtc1Tool_Click);
            // 
            // _etc1Tool
            // 
            this._etc1Tool.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._etc1Tool.Location = new System.Drawing.Point(146, 47);
            this._etc1Tool.Name = "_etc1Tool";
            this._etc1Tool.Size = new System.Drawing.Size(604, 20);
            this._etc1Tool.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "etc1Tool: ";
            // 
            // _browseTargetFolder
            // 
            this._browseTargetFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._browseTargetFolder.Location = new System.Drawing.Point(766, 86);
            this._browseTargetFolder.Name = "_browseTargetFolder";
            this._browseTargetFolder.Size = new System.Drawing.Size(75, 23);
            this._browseTargetFolder.TabIndex = 9;
            this._browseTargetFolder.Text = "Browse";
            this._browseTargetFolder.UseVisualStyleBackColor = true;
            this._browseTargetFolder.Click += new System.EventHandler(this._browseTargetFolder_Click);
            // 
            // _targetFolder
            // 
            this._targetFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._targetFolder.Location = new System.Drawing.Point(146, 88);
            this._targetFolder.Name = "_targetFolder";
            this._targetFolder.Size = new System.Drawing.Size(604, 20);
            this._targetFolder.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "terget folder: ";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // _openEtc1ToolDialog
            // 
            this._openEtc1ToolDialog.FileName = "openFileDialog2";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 148);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "status:";
            // 
            // _status
            // 
            this._status.Location = new System.Drawing.Point(143, 148);
            this._status.Name = "_status";
            this._status.Size = new System.Drawing.Size(697, 83);
            this._status.TabIndex = 11;
            this._status.Text = "...";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(852, 271);
            this.Controls.Add(this._status);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._browseTargetFolder);
            this.Controls.Add(this._targetFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._browseEtc1Tool);
            this.Controls.Add(this._etc1Tool);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this._browsePngSource);
            this.Controls.Add(this._pngSource);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Textures MipMaps Generator";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _pngSource;
        private System.Windows.Forms.Button _browsePngSource;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button _generate;
        private System.Windows.Forms.Button _browseEtc1Tool;
        private System.Windows.Forms.TextBox _etc1Tool;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button _browseTargetFolder;
        private System.Windows.Forms.TextBox _targetFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private Ookii.Dialogs.VistaFolderBrowserDialog _bowsePngSourceDialog;
        private Ookii.Dialogs.VistaFolderBrowserDialog _bowseTargetFolderDialog;
        private System.Windows.Forms.OpenFileDialog _openEtc1ToolDialog;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label _status;
    }
}

