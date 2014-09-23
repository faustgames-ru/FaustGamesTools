using FaustGames.Controls;

namespace FbxConverter
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this._openEtc1ToolDialog = new System.Windows.Forms.OpenFileDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this._convert = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this._fbxSource = new OpenFileEdit();
            this._targetFolder = new OpenFolderEdit();
            this._configurations = new System.Windows.Forms.ComboBox();
            this._configurationsProperties = new System.Windows.Forms.PropertyGrid();
            this._remove = new System.Windows.Forms.Button();
            this._fbxSourcePanel = new System.Windows.Forms.Panel();
            this._targetFolderPanel = new System.Windows.Forms.Panel();
            this._configurationsPanel = new System.Windows.Forms.Panel();
            this._propertiesPanel = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this._fbxSourcePanel.SuspendLayout();
            this._targetFolderPanel.SuspendLayout();
            this._configurationsPanel.SuspendLayout();
            this._propertiesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // _openEtc1ToolDialog
            // 
            this._openEtc1ToolDialog.FileName = "openFileDialog2";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this._convert);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 527);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(880, 37);
            this.panel1.TabIndex = 15;
            // 
            // _convert
            // 
            this._convert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._convert.Location = new System.Drawing.Point(792, 7);
            this._convert.Name = "_convert";
            this._convert.Size = new System.Drawing.Size(75, 23);
            this._convert.TabIndex = 1;
            this._convert.Text = "Convert";
            this._convert.UseVisualStyleBackColor = true;
            this._convert.Click += new System.EventHandler(this._convert_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "*.fbx source file: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 69);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "configuration:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "terget folder: ";
            // 
            // _fbxSource
            // 
            this._fbxSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._fbxSource.Filter = "fbx files (*.fbx)|*.fbx|All files (*.*)|*.*";
            this._fbxSource.Location = new System.Drawing.Point(1, 1);
            this._fbxSource.Name = "_fbxSource";
            this._fbxSource.Size = new System.Drawing.Size(756, 20);
            this._fbxSource.TabIndex = 25;
            // 
            // _targetFolder
            // 
            this._targetFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._targetFolder.Location = new System.Drawing.Point(1, 1);
            this._targetFolder.Name = "_targetFolder";
            this._targetFolder.Size = new System.Drawing.Size(756, 20);
            this._targetFolder.TabIndex = 24;
            // 
            // _configurations
            // 
            this._configurations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._configurations.FormattingEnabled = true;
            this._configurations.Location = new System.Drawing.Point(1, 1);
            this._configurations.Name = "_configurations";
            this._configurations.Size = new System.Drawing.Size(675, 21);
            this._configurations.TabIndex = 26;
            this._configurations.SelectedIndexChanged += new System.EventHandler(this._configurations_SelectedIndexChanged);
            // 
            // _configurationsProperties
            // 
            this._configurationsProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._configurationsProperties.Location = new System.Drawing.Point(1, 0);
            this._configurationsProperties.Name = "_configurationsProperties";
            this._configurationsProperties.Size = new System.Drawing.Size(878, 365);
            this._configurationsProperties.TabIndex = 27;
            // 
            // _remove
            // 
            this._remove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._remove.Location = new System.Drawing.Point(794, 65);
            this._remove.Name = "_remove";
            this._remove.Size = new System.Drawing.Size(75, 23);
            this._remove.TabIndex = 28;
            this._remove.Text = "Remove";
            this._remove.UseVisualStyleBackColor = true;
            this._remove.Click += new System.EventHandler(this._remove_Click);
            // 
            // _fbxSourcePanel
            // 
            this._fbxSourcePanel.BackColor = System.Drawing.SystemColors.Control;
            this._fbxSourcePanel.Controls.Add(this._fbxSource);
            this._fbxSourcePanel.Location = new System.Drawing.Point(111, 12);
            this._fbxSourcePanel.Name = "_fbxSourcePanel";
            this._fbxSourcePanel.Size = new System.Drawing.Size(758, 22);
            this._fbxSourcePanel.TabIndex = 29;
            // 
            // _targetFolderPanel
            // 
            this._targetFolderPanel.BackColor = System.Drawing.SystemColors.Control;
            this._targetFolderPanel.Controls.Add(this._targetFolder);
            this._targetFolderPanel.Location = new System.Drawing.Point(111, 39);
            this._targetFolderPanel.Name = "_targetFolderPanel";
            this._targetFolderPanel.Size = new System.Drawing.Size(758, 22);
            this._targetFolderPanel.TabIndex = 30;
            // 
            // _configurationsPanel
            // 
            this._configurationsPanel.BackColor = System.Drawing.SystemColors.Control;
            this._configurationsPanel.Controls.Add(this._configurations);
            this._configurationsPanel.Location = new System.Drawing.Point(110, 65);
            this._configurationsPanel.Name = "_configurationsPanel";
            this._configurationsPanel.Size = new System.Drawing.Size(677, 23);
            this._configurationsPanel.TabIndex = 31;
            // 
            // _propertiesPanel
            // 
            this._propertiesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._propertiesPanel.BackColor = System.Drawing.SystemColors.Control;
            this._propertiesPanel.Controls.Add(this._configurationsProperties);
            this._propertiesPanel.Location = new System.Drawing.Point(0, 121);
            this._propertiesPanel.Name = "_propertiesPanel";
            this._propertiesPanel.Size = new System.Drawing.Size(880, 366);
            this._propertiesPanel.TabIndex = 32;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(880, 564);
            this.Controls.Add(this._propertiesPanel);
            this.Controls.Add(this._configurationsPanel);
            this.Controls.Add(this._targetFolderPanel);
            this.Controls.Add(this._fbxSourcePanel);
            this.Controls.Add(this._remove);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "fbxConverter";
            this.panel1.ResumeLayout(false);
            this._fbxSourcePanel.ResumeLayout(false);
            this._fbxSourcePanel.PerformLayout();
            this._targetFolderPanel.ResumeLayout(false);
            this._targetFolderPanel.PerformLayout();
            this._configurationsPanel.ResumeLayout(false);
            this._propertiesPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.OpenFileDialog _openEtc1ToolDialog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button _convert;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private OpenFolderEdit _targetFolder;
        private OpenFileEdit _fbxSource;
        private System.Windows.Forms.ComboBox _configurations;
        private System.Windows.Forms.PropertyGrid _configurationsProperties;
        private System.Windows.Forms.Button _remove;
        private System.Windows.Forms.Panel _fbxSourcePanel;
        private System.Windows.Forms.Panel _targetFolderPanel;
        private System.Windows.Forms.Panel _configurationsPanel;
        private System.Windows.Forms.Panel _propertiesPanel;

    }
}

