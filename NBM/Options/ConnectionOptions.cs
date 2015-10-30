using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using WinForms = System.Windows.Forms;

using NBM.Plugin;
using NBM.Plugin.Settings;

// 26/6/03

namespace NBM.OptionNodes
{
	/// <summary>
	/// Connection options - for deciding what kind of proxy to use to connect.
	/// </summary>
	public class ConnectionOptions : System.Windows.Forms.UserControl, IOptions
	{
		private GlobalSettings settings = GlobalSettings.Instance();

		private System.Windows.Forms.GroupBox serverInfoGroupBox;
		private System.Windows.Forms.CheckBox savePasswordCheckBox;
		private System.Windows.Forms.Label passwordLabel;
		private System.Windows.Forms.TextBox passwordTextBox;
		private System.Windows.Forms.Label usernameLabel;
		private System.Windows.Forms.TextBox usernameTextBox;
		private System.Windows.Forms.GroupBox authGroupBox;
		private System.Windows.Forms.Label portLabel;
		private System.Windows.Forms.TextBox portTextBox;
		private System.Windows.Forms.Label hostLabel;
		private System.Windows.Forms.TextBox hostTextBox;
		private System.Windows.Forms.RadioButton directRadioButton;
		private System.Windows.Forms.RadioButton socks4RadioButton;
		private System.Windows.Forms.RadioButton socks5RadioButton;
		private System.Windows.Forms.RadioButton httpProxyRadioButton;
		private System.Windows.Forms.CheckBox useAuthCheckBox;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary>
		/// Constructs a ConnectionOptions object.
		/// </summary>
		public ConnectionOptions()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();


			switch (settings.InternetConnection)
			{
				default:
				case InternetConnection.Direct:
					this.directRadioButton.Checked = true;
					break;
				case InternetConnection.Socks4:
					this.socks4RadioButton.Checked = true;
					break;
				case InternetConnection.Socks5:
					this.socks5RadioButton.Checked = true;
					break;
				case InternetConnection.Http:
					this.httpProxyRadioButton.Checked = true;
					break;
			}

			this.hostTextBox.Text = this.settings.ProxyServerHost;
			this.portTextBox.Text = this.settings.ProxyServerPort.ToString();

			this.useAuthCheckBox.Checked = this.settings.UseProxyAuthentication;

			this.usernameTextBox.Text = this.settings.ProxyUsername;
			this.passwordTextBox.Text = this.settings.ProxyPassword;
			this.savePasswordCheckBox.Checked = this.settings.RememberProxyPassword;
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
			get { return "Connection"; }
		}


		/// <summary>
		/// Save settings
		/// </summary>
		public void Save()
		{
			if (this.directRadioButton.Checked)
				this.settings.InternetConnection = InternetConnection.Direct;
			else if (this.socks4RadioButton.Checked)
				this.settings.InternetConnection = InternetConnection.Socks4;
			else if (this.socks5RadioButton.Checked)
				this.settings.InternetConnection = InternetConnection.Socks5;
			else if (this.httpProxyRadioButton.Checked)
				this.settings.InternetConnection = InternetConnection.Http;
			else
				throw new ApplicationException();

			this.settings.ProxyServerHost = this.hostTextBox.Text;
			this.settings.ProxyServerPort = int.Parse(this.portTextBox.Text);

			this.settings.ProxyUsername = this.usernameTextBox.Text;
			this.settings.ProxyPassword = this.passwordTextBox.Text;
			this.settings.RememberProxyPassword = this.savePasswordCheckBox.Checked;
		}


		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.directRadioButton = new System.Windows.Forms.RadioButton();
			this.socks4RadioButton = new System.Windows.Forms.RadioButton();
			this.socks5RadioButton = new System.Windows.Forms.RadioButton();
			this.httpProxyRadioButton = new System.Windows.Forms.RadioButton();
			this.serverInfoGroupBox = new System.Windows.Forms.GroupBox();
			this.authGroupBox = new System.Windows.Forms.GroupBox();
			this.savePasswordCheckBox = new System.Windows.Forms.CheckBox();
			this.passwordLabel = new System.Windows.Forms.Label();
			this.passwordTextBox = new System.Windows.Forms.TextBox();
			this.usernameLabel = new System.Windows.Forms.Label();
			this.usernameTextBox = new System.Windows.Forms.TextBox();
			this.portLabel = new System.Windows.Forms.Label();
			this.portTextBox = new System.Windows.Forms.TextBox();
			this.hostLabel = new System.Windows.Forms.Label();
			this.hostTextBox = new System.Windows.Forms.TextBox();
			this.useAuthCheckBox = new System.Windows.Forms.CheckBox();
			this.serverInfoGroupBox.SuspendLayout();
			this.authGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// directRadioButton
			// 
			this.directRadioButton.Location = new System.Drawing.Point(8, 16);
			this.directRadioButton.Name = "directRadioButton";
			this.directRadioButton.Size = new System.Drawing.Size(384, 16);
			this.directRadioButton.TabIndex = 0;
			this.directRadioButton.Text = "Direct connection to the internet";
			this.directRadioButton.CheckedChanged += new System.EventHandler(this.directRadioButton_CheckedChanged);
			// 
			// socks4RadioButton
			// 
			this.socks4RadioButton.Location = new System.Drawing.Point(8, 40);
			this.socks4RadioButton.Name = "socks4RadioButton";
			this.socks4RadioButton.Size = new System.Drawing.Size(384, 16);
			this.socks4RadioButton.TabIndex = 1;
			this.socks4RadioButton.Text = "SOCKS v4 proxy";
			this.socks4RadioButton.CheckedChanged += new System.EventHandler(this.proxyRadioButton_CheckedChanged);
			// 
			// socks5RadioButton
			// 
			this.socks5RadioButton.Location = new System.Drawing.Point(8, 64);
			this.socks5RadioButton.Name = "socks5RadioButton";
			this.socks5RadioButton.Size = new System.Drawing.Size(384, 16);
			this.socks5RadioButton.TabIndex = 2;
			this.socks5RadioButton.Text = "SOCKS v5 proxy";
			this.socks5RadioButton.CheckedChanged += new System.EventHandler(this.proxyRadioButton_CheckedChanged);
			// 
			// httpProxyRadioButton
			// 
			this.httpProxyRadioButton.Location = new System.Drawing.Point(8, 88);
			this.httpProxyRadioButton.Name = "httpProxyRadioButton";
			this.httpProxyRadioButton.Size = new System.Drawing.Size(384, 16);
			this.httpProxyRadioButton.TabIndex = 3;
			this.httpProxyRadioButton.Text = "HTTP proxy";
			this.httpProxyRadioButton.CheckedChanged += new System.EventHandler(this.proxyRadioButton_CheckedChanged);
			// 
			// serverInfoGroupBox
			// 
			this.serverInfoGroupBox.Controls.AddRange(new System.Windows.Forms.Control[] {
																																										 this.portLabel,
																																										 this.portTextBox,
																																										 this.hostLabel,
																																										 this.hostTextBox});
			this.serverInfoGroupBox.Location = new System.Drawing.Point(8, 120);
			this.serverInfoGroupBox.Name = "serverInfoGroupBox";
			this.serverInfoGroupBox.Size = new System.Drawing.Size(384, 80);
			this.serverInfoGroupBox.TabIndex = 4;
			this.serverInfoGroupBox.TabStop = false;
			this.serverInfoGroupBox.Text = "Proxy server information";
			// 
			// authGroupBox
			// 
			this.authGroupBox.Controls.AddRange(new System.Windows.Forms.Control[] {
																																							 this.useAuthCheckBox,
																																							 this.savePasswordCheckBox,
																																							 this.passwordLabel,
																																							 this.passwordTextBox,
																																							 this.usernameLabel,
																																							 this.usernameTextBox});
			this.authGroupBox.Location = new System.Drawing.Point(8, 208);
			this.authGroupBox.Name = "authGroupBox";
			this.authGroupBox.Size = new System.Drawing.Size(384, 104);
			this.authGroupBox.TabIndex = 5;
			this.authGroupBox.TabStop = false;
			// 
			// savePasswordCheckBox
			// 
			this.savePasswordCheckBox.Location = new System.Drawing.Point(152, 80);
			this.savePasswordCheckBox.Name = "savePasswordCheckBox";
			this.savePasswordCheckBox.Size = new System.Drawing.Size(200, 16);
			this.savePasswordCheckBox.TabIndex = 15;
			this.savePasswordCheckBox.Text = "Save password";
			// 
			// passwordLabel
			// 
			this.passwordLabel.Location = new System.Drawing.Point(24, 48);
			this.passwordLabel.Name = "passwordLabel";
			this.passwordLabel.Size = new System.Drawing.Size(120, 16);
			this.passwordLabel.TabIndex = 14;
			this.passwordLabel.Text = "Password:";
			this.passwordLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// passwordTextBox
			// 
			this.passwordTextBox.Location = new System.Drawing.Point(152, 48);
			this.passwordTextBox.Name = "passwordTextBox";
			this.passwordTextBox.PasswordChar = '*';
			this.passwordTextBox.Size = new System.Drawing.Size(200, 20);
			this.passwordTextBox.TabIndex = 13;
			this.passwordTextBox.Text = "";
			// 
			// usernameLabel
			// 
			this.usernameLabel.Location = new System.Drawing.Point(24, 24);
			this.usernameLabel.Name = "usernameLabel";
			this.usernameLabel.Size = new System.Drawing.Size(120, 16);
			this.usernameLabel.TabIndex = 12;
			this.usernameLabel.Text = "Username:";
			this.usernameLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// usernameTextBox
			// 
			this.usernameTextBox.Location = new System.Drawing.Point(152, 24);
			this.usernameTextBox.Name = "usernameTextBox";
			this.usernameTextBox.Size = new System.Drawing.Size(200, 20);
			this.usernameTextBox.TabIndex = 11;
			this.usernameTextBox.Text = "";
			// 
			// portLabel
			// 
			this.portLabel.Location = new System.Drawing.Point(24, 48);
			this.portLabel.Name = "portLabel";
			this.portLabel.Size = new System.Drawing.Size(120, 16);
			this.portLabel.TabIndex = 21;
			this.portLabel.Text = "Port:";
			this.portLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// portTextBox
			// 
			this.portTextBox.Location = new System.Drawing.Point(152, 48);
			this.portTextBox.Name = "portTextBox";
			this.portTextBox.Size = new System.Drawing.Size(200, 20);
			this.portTextBox.TabIndex = 20;
			this.portTextBox.Text = "";
			// 
			// hostLabel
			// 
			this.hostLabel.Location = new System.Drawing.Point(24, 24);
			this.hostLabel.Name = "hostLabel";
			this.hostLabel.Size = new System.Drawing.Size(120, 16);
			this.hostLabel.TabIndex = 19;
			this.hostLabel.Text = "Host:";
			this.hostLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// hostTextBox
			// 
			this.hostTextBox.Location = new System.Drawing.Point(152, 24);
			this.hostTextBox.Name = "hostTextBox";
			this.hostTextBox.Size = new System.Drawing.Size(200, 20);
			this.hostTextBox.TabIndex = 18;
			this.hostTextBox.Text = "";
			// 
			// useAuthCheckBox
			// 
			this.useAuthCheckBox.Location = new System.Drawing.Point(8, 0);
			this.useAuthCheckBox.Name = "useAuthCheckBox";
			this.useAuthCheckBox.Size = new System.Drawing.Size(120, 16);
			this.useAuthCheckBox.TabIndex = 16;
			this.useAuthCheckBox.Text = "Use authentication";
			this.useAuthCheckBox.CheckedChanged += new System.EventHandler(this.useAuthCheckBox_CheckedChanged);
			// 
			// ConnectionOptions
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.authGroupBox,
																																	this.serverInfoGroupBox,
																																	this.httpProxyRadioButton,
																																	this.socks5RadioButton,
																																	this.socks4RadioButton,
																																	this.directRadioButton});
			this.Name = "ConnectionOptions";
			this.Size = new System.Drawing.Size(400, 320);
			this.serverInfoGroupBox.ResumeLayout(false);
			this.authGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void directRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			// disable proxy options
			if (this.directRadioButton.Checked)
				this.authGroupBox.Enabled = this.serverInfoGroupBox.Enabled = false;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void proxyRadioButton_CheckedChanged(object sender, System.EventArgs e)
		{
			WinForms.RadioButton radio = (WinForms.RadioButton)sender;

			// enable proxy options
			if (radio.Checked)
				this.authGroupBox.Enabled = this.serverInfoGroupBox.Enabled = true;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void useAuthCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			// enable username & password if auth check box is clicked
			this.usernameTextBox.Enabled = this.passwordTextBox.Enabled = this.useAuthCheckBox.Checked;
		}
	}
}
