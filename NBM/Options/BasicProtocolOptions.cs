using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using NBM.Plugin;

// 26/6/03

namespace NBM.OptionNodes
{
	/// <summary>
	/// Basic protocol options - stuff like username, password and server information
	/// </summary>
	public class BasicProtocolOptions : System.Windows.Forms.UserControl, IOptions
	{
		private ProtocolSettings settings;
		private System.Windows.Forms.TextBox usernameTextBox;
		private System.Windows.Forms.Label usernameLabel;
		private System.Windows.Forms.Label passwordLabel;
		private System.Windows.Forms.TextBox passwordTextBox;
		private System.Windows.Forms.CheckBox savePasswordCheckBox;
		private System.Windows.Forms.GroupBox authenticationGroupBox;
		private System.Windows.Forms.GroupBox serverInfoGroupBox;
		private System.Windows.Forms.Label portLabel;
		private System.Windows.Forms.Label hostLabel;
		private System.Windows.Forms.TextBox portTextBox;
		private System.Windows.Forms.TextBox hostTextBox;
		private System.Windows.Forms.CheckBox enableCheckBox;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary>
		/// Constructs a basic protocol options object.
		/// </summary>
		/// <param name="settings"></param>
		public BasicProtocolOptions(ProtocolSettings settings)
		{
			this.settings = settings;

			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// load up defaults
			this.enableCheckBox.Checked = settings.Enabled;
			this.usernameTextBox.Text = settings.Username;
			this.passwordTextBox.Text = settings.Password;
			this.savePasswordCheckBox.Checked = settings.RememberPassword;

			this.hostTextBox.Text = settings.ServerHost;
			this.portTextBox.Text = settings.ServerPort.ToString();
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


		/// <summary>
		/// Name of node
		/// </summary>
		public string NodeName
		{
			get { return "Basic"; }
		}


		/// <summary>
		/// Saves the information from the form to the ProtocolSettings object.
		/// </summary>
		public void Save()
		{
			this.settings.Enabled = this.enableCheckBox.Checked;
			this.settings.Username = this.usernameTextBox.Text;
			this.settings.Password = this.passwordTextBox.Text;
			this.settings.RememberPassword = this.savePasswordCheckBox.Checked;

			this.settings.ServerHost = this.hostTextBox.Text;
			this.settings.ServerPort = int.Parse(this.portTextBox.Text);
		}


		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.authenticationGroupBox = new System.Windows.Forms.GroupBox();
			this.savePasswordCheckBox = new System.Windows.Forms.CheckBox();
			this.passwordLabel = new System.Windows.Forms.Label();
			this.passwordTextBox = new System.Windows.Forms.TextBox();
			this.usernameLabel = new System.Windows.Forms.Label();
			this.usernameTextBox = new System.Windows.Forms.TextBox();
			this.enableCheckBox = new System.Windows.Forms.CheckBox();
			this.serverInfoGroupBox = new System.Windows.Forms.GroupBox();
			this.portLabel = new System.Windows.Forms.Label();
			this.portTextBox = new System.Windows.Forms.TextBox();
			this.hostLabel = new System.Windows.Forms.Label();
			this.hostTextBox = new System.Windows.Forms.TextBox();
			this.authenticationGroupBox.SuspendLayout();
			this.serverInfoGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// authenticationGroupBox
			// 
			this.authenticationGroupBox.Controls.AddRange(new System.Windows.Forms.Control[] {
																																												 this.savePasswordCheckBox,
																																												 this.passwordLabel,
																																												 this.passwordTextBox,
																																												 this.usernameLabel,
																																												 this.usernameTextBox});
			this.authenticationGroupBox.Location = new System.Drawing.Point(8, 32);
			this.authenticationGroupBox.Name = "authenticationGroupBox";
			this.authenticationGroupBox.Size = new System.Drawing.Size(384, 104);
			this.authenticationGroupBox.TabIndex = 0;
			this.authenticationGroupBox.TabStop = false;
			this.authenticationGroupBox.Text = "Authentication";
			// 
			// savePasswordCheckBox
			// 
			this.savePasswordCheckBox.Location = new System.Drawing.Point(144, 80);
			this.savePasswordCheckBox.Name = "savePasswordCheckBox";
			this.savePasswordCheckBox.Size = new System.Drawing.Size(200, 16);
			this.savePasswordCheckBox.TabIndex = 5;
			this.savePasswordCheckBox.Text = "Save password";
			// 
			// passwordLabel
			// 
			this.passwordLabel.Location = new System.Drawing.Point(16, 48);
			this.passwordLabel.Name = "passwordLabel";
			this.passwordLabel.Size = new System.Drawing.Size(120, 16);
			this.passwordLabel.TabIndex = 4;
			this.passwordLabel.Text = "Password:";
			this.passwordLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// passwordTextBox
			// 
			this.passwordTextBox.Location = new System.Drawing.Point(144, 48);
			this.passwordTextBox.Name = "passwordTextBox";
			this.passwordTextBox.PasswordChar = '*';
			this.passwordTextBox.Size = new System.Drawing.Size(200, 20);
			this.passwordTextBox.TabIndex = 3;
			this.passwordTextBox.Text = "";
			// 
			// usernameLabel
			// 
			this.usernameLabel.Location = new System.Drawing.Point(16, 24);
			this.usernameLabel.Name = "usernameLabel";
			this.usernameLabel.Size = new System.Drawing.Size(120, 16);
			this.usernameLabel.TabIndex = 2;
			this.usernameLabel.Text = "Username:";
			this.usernameLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// usernameTextBox
			// 
			this.usernameTextBox.Location = new System.Drawing.Point(144, 24);
			this.usernameTextBox.Name = "usernameTextBox";
			this.usernameTextBox.Size = new System.Drawing.Size(200, 20);
			this.usernameTextBox.TabIndex = 1;
			this.usernameTextBox.Text = "";
			// 
			// enableCheckBox
			// 
			this.enableCheckBox.BackColor = System.Drawing.SystemColors.Control;
			this.enableCheckBox.Checked = true;
			this.enableCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
			this.enableCheckBox.Location = new System.Drawing.Point(8, 8);
			this.enableCheckBox.Name = "enableCheckBox";
			this.enableCheckBox.Size = new System.Drawing.Size(96, 16);
			this.enableCheckBox.TabIndex = 1;
			this.enableCheckBox.Text = " Enable";
			this.enableCheckBox.CheckedChanged += new System.EventHandler(this.enableCheckBox_CheckedChanged);
			// 
			// serverInfoGroupBox
			// 
			this.serverInfoGroupBox.Controls.AddRange(new System.Windows.Forms.Control[] {
																																										 this.portLabel,
																																										 this.portTextBox,
																																										 this.hostLabel,
																																										 this.hostTextBox});
			this.serverInfoGroupBox.Location = new System.Drawing.Point(8, 152);
			this.serverInfoGroupBox.Name = "serverInfoGroupBox";
			this.serverInfoGroupBox.Size = new System.Drawing.Size(384, 80);
			this.serverInfoGroupBox.TabIndex = 14;
			this.serverInfoGroupBox.TabStop = false;
			this.serverInfoGroupBox.Text = "Server Information";
			// 
			// portLabel
			// 
			this.portLabel.Location = new System.Drawing.Point(16, 48);
			this.portLabel.Name = "portLabel";
			this.portLabel.Size = new System.Drawing.Size(120, 16);
			this.portLabel.TabIndex = 17;
			this.portLabel.Text = "Port:";
			this.portLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// portTextBox
			// 
			this.portTextBox.Location = new System.Drawing.Point(144, 48);
			this.portTextBox.Name = "portTextBox";
			this.portTextBox.Size = new System.Drawing.Size(200, 20);
			this.portTextBox.TabIndex = 16;
			this.portTextBox.Text = "";
			// 
			// hostLabel
			// 
			this.hostLabel.Location = new System.Drawing.Point(16, 24);
			this.hostLabel.Name = "hostLabel";
			this.hostLabel.Size = new System.Drawing.Size(120, 16);
			this.hostLabel.TabIndex = 15;
			this.hostLabel.Text = "Host:";
			this.hostLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// hostTextBox
			// 
			this.hostTextBox.Location = new System.Drawing.Point(144, 24);
			this.hostTextBox.Name = "hostTextBox";
			this.hostTextBox.Size = new System.Drawing.Size(200, 20);
			this.hostTextBox.TabIndex = 14;
			this.hostTextBox.Text = "";
			// 
			// BasicProtocolOptions
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.serverInfoGroupBox,
																																	this.enableCheckBox,
																																	this.authenticationGroupBox});
			this.Name = "BasicProtocolOptions";
			this.Size = new System.Drawing.Size(400, 320);
			this.authenticationGroupBox.ResumeLayout(false);
			this.serverInfoGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void enableCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			this.authenticationGroupBox.Enabled =
				this.serverInfoGroupBox.Enabled = this.enableCheckBox.Checked;
		}
	}
}
