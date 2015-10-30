using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NBM.Plugin;

using WinForms = System.Windows.Forms;

// 23/5/03

namespace NBM
{
	/// <summary>
	/// Form for when authentication fails when trying to connect.
	/// </summary>
	public class AuthFailedForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox usernameBox;
		private System.Windows.Forms.TextBox passwordBox;
		private System.Windows.Forms.Button loginButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label authLabel;
		private System.Windows.Forms.CheckBox rememberPasswordBox;

		private Protocol protocol;


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary>
		/// Constructs an AuthFailedForm.
		/// </summary>
		/// <param name="protocol">Associated protocol</param>
		/// <param name="furtherInformation">Any further information that can be provided</param>
		public AuthFailedForm(Protocol protocol, string furtherInformation)
		{
			this.protocol = protocol;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.Text = protocol.Settings.Constants.Name + " authentication failed";
			this.usernameBox.Text = protocol.Settings.Username;
			this.passwordBox.Text = protocol.Settings.Password;
			this.rememberPasswordBox.Checked = protocol.Settings.RememberPassword;
			this.authLabel.Text = furtherInformation;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.usernameBox = new System.Windows.Forms.TextBox();
			this.passwordBox = new System.Windows.Forms.TextBox();
			this.loginButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.authLabel = new System.Windows.Forms.Label();
			this.rememberPasswordBox = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Username:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 80);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "Password:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// usernameBox
			// 
			this.usernameBox.Location = new System.Drawing.Point(80, 56);
			this.usernameBox.Name = "usernameBox";
			this.usernameBox.Size = new System.Drawing.Size(200, 20);
			this.usernameBox.TabIndex = 2;
			this.usernameBox.Text = "";
			// 
			// passwordBox
			// 
			this.passwordBox.Location = new System.Drawing.Point(80, 80);
			this.passwordBox.Name = "passwordBox";
			this.passwordBox.PasswordChar = '*';
			this.passwordBox.Size = new System.Drawing.Size(200, 20);
			this.passwordBox.TabIndex = 3;
			this.passwordBox.Text = "";
			// 
			// loginButton
			// 
			this.loginButton.Location = new System.Drawing.Point(64, 144);
			this.loginButton.Name = "loginButton";
			this.loginButton.Size = new System.Drawing.Size(72, 24);
			this.loginButton.TabIndex = 4;
			this.loginButton.Text = "Log in";
			this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(160, 144);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(72, 24);
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// authLabel
			// 
			this.authLabel.Location = new System.Drawing.Point(8, 8);
			this.authLabel.Name = "authLabel";
			this.authLabel.Size = new System.Drawing.Size(280, 40);
			this.authLabel.TabIndex = 6;
			this.authLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// rememberPasswordBox
			// 
			this.rememberPasswordBox.Location = new System.Drawing.Point(80, 112);
			this.rememberPasswordBox.Name = "rememberPasswordBox";
			this.rememberPasswordBox.Size = new System.Drawing.Size(200, 16);
			this.rememberPasswordBox.TabIndex = 7;
			this.rememberPasswordBox.Text = "Remember password";
			// 
			// AuthFailedForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(298, 183);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.rememberPasswordBox,
																																	this.authLabel,
																																	this.cancelButton,
																																	this.loginButton,
																																	this.passwordBox,
																																	this.usernameBox,
																																	this.label2,
																																	this.label1});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "AuthFailedForm";
			this.Text = "Connection Failed";
			this.ResumeLayout(false);

		}
		#endregion


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void loginButton_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = WinForms.DialogResult.OK;

			this.protocol.Settings.Username = this.usernameBox.Text;
			this.protocol.Settings.Password = this.passwordBox.Text;
			this.protocol.Settings.RememberPassword = this.rememberPasswordBox.Checked;

			this.Close();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			this.DialogResult = WinForms.DialogResult.Cancel;
			this.Close();
		}
	}
}
