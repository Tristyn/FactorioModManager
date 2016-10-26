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
            this.SuspendLayout();
            // 
            // Status
            // 
            this.Status.AutoSize = true;
            this.Status.Location = new System.Drawing.Point(-3, 16);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(45, 13);
            this.Status.TabIndex = 0;
            this.Status.Text = "{Status}";
            // 
            // Spec
            // 
            this.Spec.AutoSize = true;
            this.Spec.Location = new System.Drawing.Point(-3, 0);
            this.Spec.Name = "Spec";
            this.Spec.Size = new System.Drawing.Size(45, 13);
            this.Spec.TabIndex = 1;
            this.Spec.Text = "{Status}";
            // 
            // InstallWebBtn
            // 
            this.InstallWebBtn.Location = new System.Drawing.Point(0, 33);
            this.InstallWebBtn.Name = "InstallWebBtn";
            this.InstallWebBtn.Size = new System.Drawing.Size(76, 23);
            this.InstallWebBtn.TabIndex = 2;
            this.InstallWebBtn.Text = "Install (Web)";
            this.InstallWebBtn.UseVisualStyleBackColor = true;
            // 
            // InstallArchiveBtn
            // 
            this.InstallArchiveBtn.Location = new System.Drawing.Point(82, 33);
            this.InstallArchiveBtn.Name = "InstallArchiveBtn";
            this.InstallArchiveBtn.Size = new System.Drawing.Size(66, 23);
            this.InstallArchiveBtn.TabIndex = 3;
            this.InstallArchiveBtn.Text = "Install (Zip)";
            this.InstallArchiveBtn.UseVisualStyleBackColor = true;
            // 
            // InstallationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.InstallArchiveBtn);
            this.Controls.Add(this.InstallWebBtn);
            this.Controls.Add(this.Spec);
            this.Controls.Add(this.Status);
            this.Name = "InstallationView";
            this.Size = new System.Drawing.Size(157, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Status;
        private System.Windows.Forms.Label Spec;
        private System.Windows.Forms.Button InstallWebBtn;
        private System.Windows.Forms.Button InstallArchiveBtn;
    }
}
