namespace FxCodeGenerator
{
    partial class Main
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
            this._fxEffectsFolder = new FaustGames.Controls.OpenFolderEdit();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this._convert = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this._jsEffectsFolder = new FaustGames.Controls.OpenFolderEdit();
            this.label3 = new System.Windows.Forms.Label();
            this._javaEffecsFolder = new FaustGames.Controls.OpenFolderEdit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _fxEffectsFolder
            // 
            this._fxEffectsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._fxEffectsFolder.Location = new System.Drawing.Point(138, 12);
            this._fxEffectsFolder.Name = "_fxEffectsFolder";
            this._fxEffectsFolder.Size = new System.Drawing.Size(784, 20);
            this._fxEffectsFolder.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "effects folder:";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this._convert);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 123);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(934, 37);
            this.panel1.TabIndex = 16;
            // 
            // _convert
            // 
            this._convert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._convert.Location = new System.Drawing.Point(796, 7);
            this._convert.Name = "_convert";
            this._convert.Size = new System.Drawing.Size(125, 23);
            this._convert.TabIndex = 1;
            this._convert.Text = "Generate code";
            this._convert.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "js effect folder:";
            // 
            // _jsEffectsFolder
            // 
            this._jsEffectsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._jsEffectsFolder.Location = new System.Drawing.Point(138, 41);
            this._jsEffectsFolder.Name = "_jsEffectsFolder";
            this._jsEffectsFolder.Size = new System.Drawing.Size(784, 20);
            this._jsEffectsFolder.TabIndex = 17;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "java effects folder:";
            // 
            // _javaEffecsFolder
            // 
            this._javaEffecsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._javaEffecsFolder.Location = new System.Drawing.Point(138, 69);
            this._javaEffecsFolder.Name = "_javaEffecsFolder";
            this._javaEffecsFolder.Size = new System.Drawing.Size(784, 20);
            this._javaEffecsFolder.TabIndex = 19;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(934, 160);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._javaEffecsFolder);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._jsEffectsFolder);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._fxEffectsFolder);
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Effexts Code Generator";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FaustGames.Controls.OpenFolderEdit _fxEffectsFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button _convert;
        private System.Windows.Forms.Label label2;
        private FaustGames.Controls.OpenFolderEdit _jsEffectsFolder;
        private System.Windows.Forms.Label label3;
        private FaustGames.Controls.OpenFolderEdit _javaEffecsFolder;
    }
}

