namespace FactorioModManager.UI.Views
{
    partial class InstallationView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Status = new System.Windows.Forms.Label();
            this.Spec = new System.Windows.Forms.Label();
            this.InstallWebBtn = new System.Windows.Forms.Button();
            this.InstallArchiveBtn = new System.Windows.Forms.Button();
            this.RefreshBtn = new System.Windows.Forms.Button();
            this.PlayBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Status
            // 
            this.Status.AutoSize = true;
            this.Status.Location = new System.Drawing.Point(3, 16);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(45, 13);
            this.Status.TabIndex = 0;
            this.Status.Text = "{Status}";
            // 
            // Spec
            // 
            this.Spec.AutoSize = true;
            this.Spec.Location = new System.Drawing.Point(3, 0);
            this.Spec.Name = "Spec";
            this.Spec.Size = new System.Drawing.Size(40, 13);
            this.Spec.TabIndex = 1;
            this.Spec.Text = "{Spec}";
            // 
            // InstallWebBtn
            // 
            this.InstallWebBtn.Location = new System.Drawing.Point(3, 63);
            this.InstallWebBtn.Name = "InstallWebBtn";
            this.InstallWebBtn.Size = new System.Drawing.Size(127, 23);
            this.InstallWebBtn.TabIndex = 2;
            this.InstallWebBtn.Text = "Install (Web)";
            this.InstallWebBtn.UseVisualStyleBackColor = true;
            // 
            // InstallArchiveBtn
            // 
            this.InstallArchiveBtn.Location = new System.Drawing.Point(3, 92);
            this.InstallArchiveBtn.Name = "InstallArchiveBtn";
            this.InstallArchiveBtn.Size = new System.Drawing.Size(127, 23);
            this.InstallArchiveBtn.TabIndex = 3;
            this.InstallArchiveBtn.Text = "Install (Zip)";
            this.InstallArchiveBtn.UseVisualStyleBackColor = true;
            // 
            // RefreshBtn
            // 
            this.RefreshBtn.Location = new System.Drawing.Point(3, 32);
            this.RefreshBtn.Name = "RefreshBtn";
            this.RefreshBtn.Size = new System.Drawing.Size(60, 23);
            this.RefreshBtn.TabIndex = 4;
            this.RefreshBtn.Text = "Refresh";
            this.RefreshBtn.UseVisualStyleBackColor = true;
            // 
            // PlayBtn
            // 
            this.PlayBtn.Location = new System.Drawing.Point(69, 32);
            this.PlayBtn.Name = "PlayBtn";
            this.PlayBtn.Size = new System.Drawing.Size(61, 23);
            this.PlayBtn.TabIndex = 5;
            this.PlayBtn.Text = "Play";
            this.PlayBtn.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label1.Location = new System.Drawing.Point(3, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 2);
            this.label1.TabIndex = 6;
            // 
            // InstallationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.PlayBtn);
            this.Controls.Add(this.RefreshBtn);
            this.Controls.Add(this.InstallArchiveBtn);
            this.Controls.Add(this.InstallWebBtn);
            this.Controls.Add(this.Spec);
            this.Controls.Add(this.Status);
            this.Name = "InstallationView";
            this.Size = new System.Drawing.Size(133, 123);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Status;
        private System.Windows.Forms.Label Spec;
        private System.Windows.Forms.Button InstallWebBtn;
        private System.Windows.Forms.Button InstallArchiveBtn;
        private System.Windows.Forms.Button RefreshBtn;
        private System.Windows.Forms.Button PlayBtn;
        private System.Windows.Forms.Label label1;
    }
}
