namespace FactorioModManager.UI.Views
{
    partial class SignInView
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
            this.UsernameOrEmail = new FactorioModManager.UI.Controls.CueTextBox();
            this.Password = new FactorioModManager.UI.Controls.CueTextBox();
            this.SignInBtn = new System.Windows.Forms.Button();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.ErrorLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sign in using your Factorio account to access this feature";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // UsernameOrEmail
            // 
            this.UsernameOrEmail.Cue = "Username or Email Address";
            this.UsernameOrEmail.Location = new System.Drawing.Point(12, 76);
            this.UsernameOrEmail.Name = "UsernameOrEmail";
            this.UsernameOrEmail.Size = new System.Drawing.Size(187, 20);
            this.UsernameOrEmail.TabIndex = 1;
            // 
            // Password
            // 
            this.Password.Cue = "Password";
            this.Password.Location = new System.Drawing.Point(12, 102);
            this.Password.Name = "Password";
            this.Password.Size = new System.Drawing.Size(187, 20);
            this.Password.TabIndex = 2;
            this.Password.UseSystemPasswordChar = true;
            // 
            // SignInBtn
            // 
            this.SignInBtn.Enabled = false;
            this.SignInBtn.Location = new System.Drawing.Point(124, 138);
            this.SignInBtn.Name = "SignInBtn";
            this.SignInBtn.Size = new System.Drawing.Size(75, 23);
            this.SignInBtn.TabIndex = 3;
            this.SignInBtn.Text = "Sign In";
            this.SignInBtn.UseVisualStyleBackColor = true;
            // 
            // CancelBtn
            // 
            this.CancelBtn.Location = new System.Drawing.Point(12, 138);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(75, 23);
            this.CancelBtn.TabIndex = 4;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            // 
            // ErrorLabel
            // 
            this.ErrorLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.ErrorLabel.Location = new System.Drawing.Point(12, 46);
            this.ErrorLabel.Name = "ErrorLabel";
            this.ErrorLabel.Size = new System.Drawing.Size(187, 18);
            this.ErrorLabel.TabIndex = 5;
            this.ErrorLabel.Text = "label2";
            this.ErrorLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ErrorLabel.Visible = false;
            // 
            // SignInForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(211, 173);
            this.Controls.Add(this.ErrorLabel);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.SignInBtn);
            this.Controls.Add(this.Password);
            this.Controls.Add(this.UsernameOrEmail);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SignInForm";
            this.Text = "SignInForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Controls.CueTextBox UsernameOrEmail;
        private Controls.CueTextBox Password;
        private System.Windows.Forms.Button SignInBtn;
        private System.Windows.Forms.Button CancelBtn;
        private System.Windows.Forms.Label ErrorLabel;
    }
}