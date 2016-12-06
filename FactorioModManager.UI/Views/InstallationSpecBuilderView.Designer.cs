namespace FactorioModManager.UI.Views
{
    partial class InstallationSpecBuilderView
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
            this.label1 = new System.Windows.Forms.Label();
            this.CpuArchitecture = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.BuildConfiguration = new System.Windows.Forms.ComboBox();
            this.OkButton = new System.Windows.Forms.Button();
            this.VersionMajor = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.VersionMinor = new System.Windows.Forms.TextBox();
            this.VersionRevision = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Version:";
            // 
            // CpuArchitecture
            // 
            this.CpuArchitecture.FormattingEnabled = true;
            this.CpuArchitecture.Items.AddRange(new object[] {
            "64 Bit",
            "32 Bit"});
            this.CpuArchitecture.Location = new System.Drawing.Point(84, 38);
            this.CpuArchitecture.Name = "CpuArchitecture";
            this.CpuArchitecture.Size = new System.Drawing.Size(121, 21);
            this.CpuArchitecture.TabIndex = 4;
            this.CpuArchitecture.SelectedIndexChanged += new System.EventHandler(this.ValidateHandler);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Cpu Arch:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Build Config:";
            // 
            // BuildConfiguration
            // 
            this.BuildConfiguration.FormattingEnabled = true;
            this.BuildConfiguration.Items.AddRange(new object[] {
            "Client",
            "Server",
            "Demo"});
            this.BuildConfiguration.Location = new System.Drawing.Point(84, 65);
            this.BuildConfiguration.Name = "BuildConfiguration";
            this.BuildConfiguration.Size = new System.Drawing.Size(121, 21);
            this.BuildConfiguration.TabIndex = 5;
            this.BuildConfiguration.SelectedIndexChanged += new System.EventHandler(this.ValidateHandler);
            // 
            // OkButton
            // 
            this.OkButton.Location = new System.Drawing.Point(128, 92);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(75, 23);
            this.OkButton.TabIndex = 6;
            this.OkButton.Text = "Submit";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.SubmitHandler);
            // 
            // VersionMajor
            // 
            this.VersionMajor.Location = new System.Drawing.Point(84, 12);
            this.VersionMajor.Name = "VersionMajor";
            this.VersionMajor.Size = new System.Drawing.Size(20, 20);
            this.VersionMajor.TabIndex = 1;
            this.VersionMajor.TextChanged += new System.EventHandler(this.ValidateHandler);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(104, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(10, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = ".";
            // 
            // VersionMinor
            // 
            this.VersionMinor.Location = new System.Drawing.Point(112, 12);
            this.VersionMinor.Name = "VersionMinor";
            this.VersionMinor.Size = new System.Drawing.Size(20, 20);
            this.VersionMinor.TabIndex = 2;
            this.VersionMinor.TextChanged += new System.EventHandler(this.ValidateHandler);
            // 
            // VersionRevision
            // 
            this.VersionRevision.Location = new System.Drawing.Point(140, 12);
            this.VersionRevision.Name = "VersionRevision";
            this.VersionRevision.Size = new System.Drawing.Size(20, 20);
            this.VersionRevision.TabIndex = 3;
            this.VersionRevision.TextChanged += new System.EventHandler(this.ValidateHandler);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(132, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(10, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = ".";
            // 
            // InstallationSpecBuilderView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(215, 127);
            this.Controls.Add(this.VersionRevision);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.VersionMinor);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.VersionMajor);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.BuildConfiguration);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CpuArchitecture);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InstallationSpecBuilderView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Add New Installation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox CpuArchitecture;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox BuildConfiguration;
        private System.Windows.Forms.Button OkButton;
        private System.Windows.Forms.TextBox VersionMajor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox VersionMinor;
        private System.Windows.Forms.TextBox VersionRevision;
        private System.Windows.Forms.Label label5;
    }
}