namespace FactorioModManager.UI.Views
{
    partial class DebugShellView
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
            this.InstallationsList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.InstallationView = new FactorioModManager.UI.Views.InstallationView();
            this.SuspendLayout();
            // 
            // InstallationsList
            // 
            this.InstallationsList.FormattingEnabled = true;
            this.InstallationsList.Location = new System.Drawing.Point(12, 26);
            this.InstallationsList.Name = "InstallationsList";
            this.InstallationsList.Size = new System.Drawing.Size(133, 160);
            this.InstallationsList.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Games";
            // 
            // InstallationView
            // 
            this.InstallationView.Location = new System.Drawing.Point(12, 192);
            this.InstallationView.Name = "InstallationView";
            this.InstallationView.Size = new System.Drawing.Size(133, 123);
            this.InstallationView.TabIndex = 7;
            this.InstallationView.ViewModel = null;
            // 
            // DebugShellView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 317);
            this.Controls.Add(this.InstallationView);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.InstallationsList);
            this.Name = "DebugShellView";
            this.Text = "Factorio Mod Manager - Debug Shell";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListBox InstallationsList;
        private System.Windows.Forms.Label label2;
        private InstallationView InstallationView;
    }
}