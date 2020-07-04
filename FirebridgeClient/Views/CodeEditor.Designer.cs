namespace FirebridgeClient.Views
{
    partial class CodeEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CodeEditor));
            this._tb_code = new System.Windows.Forms.TextBox();
            this._EntryPointTB = new System.Windows.Forms.TextBox();
            this._tb_references = new System.Windows.Forms.TextBox();
            this.l_entryPoint = new System.Windows.Forms.Label();
            this._l_references = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _tb_code
            // 
            this._tb_code.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tb_code.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(77)))), ((int)(((byte)(122)))));
            this._tb_code.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(241)))), ((int)(((byte)(227)))));
            this._tb_code.Location = new System.Drawing.Point(12, 12);
            this._tb_code.Multiline = true;
            this._tb_code.Name = "_tb_code";
            this._tb_code.Size = new System.Drawing.Size(1025, 516);
            this._tb_code.TabIndex = 0;
            this._tb_code.Text = resources.GetString("_tb_code.Text");
            // 
            // _EntryPointTB
            // 
            this._EntryPointTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._EntryPointTB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(77)))), ((int)(((byte)(122)))));
            this._EntryPointTB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(241)))), ((int)(((byte)(227)))));
            this._EntryPointTB.Location = new System.Drawing.Point(104, 534);
            this._EntryPointTB.Name = "_EntryPointTB";
            this._EntryPointTB.Size = new System.Drawing.Size(175, 26);
            this._EntryPointTB.TabIndex = 1;
            this._EntryPointTB.Text = "TestApp.Program";
            // 
            // _tb_references
            // 
            this._tb_references.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._tb_references.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(77)))), ((int)(((byte)(122)))));
            this._tb_references.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(241)))), ((int)(((byte)(227)))));
            this._tb_references.Location = new System.Drawing.Point(387, 534);
            this._tb_references.Name = "_tb_references";
            this._tb_references.Size = new System.Drawing.Size(650, 26);
            this._tb_references.TabIndex = 2;
            this._tb_references.Text = "System.dll;FirebridgeShared.dll;netstandard.dll;System.Core.dll";
            // 
            // l_entryPoint
            // 
            this.l_entryPoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.l_entryPoint.AutoSize = true;
            this.l_entryPoint.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(241)))), ((int)(((byte)(227)))));
            this.l_entryPoint.Location = new System.Drawing.Point(12, 537);
            this.l_entryPoint.Name = "l_entryPoint";
            this.l_entryPoint.Size = new System.Drawing.Size(86, 20);
            this.l_entryPoint.TabIndex = 3;
            this.l_entryPoint.Text = "EntryPoint:";
            // 
            // _l_references
            // 
            this._l_references.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._l_references.AutoSize = true;
            this._l_references.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(241)))), ((int)(((byte)(227)))));
            this._l_references.Location = new System.Drawing.Point(285, 537);
            this._l_references.Name = "_l_references";
            this._l_references.Size = new System.Drawing.Size(96, 20);
            this._l_references.TabIndex = 4;
            this._l_references.Text = "References:";
            // 
            // CodeEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(84)))));
            this.ClientSize = new System.Drawing.Size(1049, 572);
            this.Controls.Add(this._l_references);
            this.Controls.Add(this.l_entryPoint);
            this.Controls.Add(this._tb_references);
            this.Controls.Add(this._EntryPointTB);
            this.Controls.Add(this._tb_code);
            this.Name = "CodeEditor";
            this.Text = "CodeEditor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CodeEditor_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label l_entryPoint;
        private System.Windows.Forms.Label _l_references;
        public System.Windows.Forms.TextBox _tb_code;
        public System.Windows.Forms.TextBox _EntryPointTB;
        public System.Windows.Forms.TextBox _tb_references;
    }
}