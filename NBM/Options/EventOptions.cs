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
	/// Options on events. Like when a friend comes online or a message is received.
	/// </summary>
	public class EventOptions : System.Windows.Forms.UserControl, IOptions
	{
		private GlobalSettings settings = GlobalSettings.Instance();

		private System.Windows.Forms.GroupBox incomingMessageGroupBox;
		private System.Windows.Forms.GroupBox friendOnlineGroupBox;
		private System.Windows.Forms.Label flashWindowLabel;
		private System.Windows.Forms.TextBox flashWindowTextBox;
		private System.Windows.Forms.RadioButton incomingMessageFlashWindowRadioButton;
		private System.Windows.Forms.RadioButton incomingMessageDoNothingRadioButton;
		private System.Windows.Forms.CheckBox incomingMessagePlaySoundCheckBox;
		private System.Windows.Forms.TextBox incomingMessageSoundPathTextBox;
		private System.Windows.Forms.Button incomingMessageSoundBrowseButton;
		private System.Windows.Forms.RadioButton friendOnlineDoNothingRadioButton;
		private System.Windows.Forms.RadioButton friendOnlineMsnPopupRadioButton;
		private System.Windows.Forms.RadioButton friendOnlineBalloonTooltipRadioButton;
		private System.Windows.Forms.CheckBox friendOnlinePlaySoundCheckBox;
		private System.Windows.Forms.Button friendOnlineSoundBrowseButton;
		private System.Windows.Forms.TextBox friendOnlineSoundPathTextBox;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary>
		/// Constructs an EventOptions object.
		/// </summary>
		public EventOptions()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();


			switch (this.settings.FriendMessageEvent)
			{
				case FriendMessageEvent.DoNothing:
					this.incomingMessageDoNothingRadioButton.Checked = true;
					break;
				case FriendMessageEvent.FlashWindow:
					this.incomingMessageFlashWindowRadioButton.Checked = true;
					break;
			}

			this.flashWindowTextBox.Text = this.settings.NumTimesToFlashOnMessageReceived.ToString();
			this.incomingMessagePlaySoundCheckBox.Checked = this.settings.PlaySoundOnMessageReceived;
			this.incomingMessageSoundPathTextBox.Text = this.settings.SoundOnMessageReceived;


			switch (this.settings.FriendOnlineEvent)
			{
				case FriendOnlineEvent.DoNothing:
					this.friendOnlineDoNothingRadioButton.Checked = true;
					break;
				case FriendOnlineEvent.BalloonToolTip:
					this.friendOnlineBalloonTooltipRadioButton.Checked = true;
					break;
				case FriendOnlineEvent.MSNPopUp:
					this.friendOnlineMsnPopupRadioButton.Checked = true;
					break;
			}

			this.friendOnlinePlaySoundCheckBox.Checked = this.settings.PlaySoundOnFriendOnline;
			this.friendOnlineSoundPathTextBox.Text = this.settings.SoundOnFriendOnline;
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
			get { return "Events"; }
		}


		/// <summary>
		/// Saves settings
		/// </summary>
		public void Save()
		{
			if (this.incomingMessageDoNothingRadioButton.Checked)
				this.settings.FriendMessageEvent = FriendMessageEvent.DoNothing;
			else if (this.incomingMessageFlashWindowRadioButton.Checked)
				this.settings.FriendMessageEvent = FriendMessageEvent.FlashWindow;

			this.settings.NumTimesToFlashOnMessageReceived = int.Parse(this.flashWindowTextBox.Text);
			this.settings.PlaySoundOnMessageReceived = this.incomingMessagePlaySoundCheckBox.Checked;
			this.settings.SoundOnMessageReceived = this.incomingMessageSoundPathTextBox.Text;

			if (this.friendOnlineDoNothingRadioButton.Checked)
				this.settings.FriendOnlineEvent = FriendOnlineEvent.DoNothing;
			else if (this.friendOnlineBalloonTooltipRadioButton.Checked)
				this.settings.FriendOnlineEvent = FriendOnlineEvent.BalloonToolTip;
			else if (this.friendOnlineMsnPopupRadioButton.Checked)
				this.settings.FriendOnlineEvent = FriendOnlineEvent.MSNPopUp;

			this.settings.PlaySoundOnFriendOnline = this.friendOnlinePlaySoundCheckBox.Checked;
			this.settings.SoundOnFriendOnline = this.friendOnlineSoundPathTextBox.Text;
		}


		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.incomingMessageGroupBox = new System.Windows.Forms.GroupBox();
			this.flashWindowLabel = new System.Windows.Forms.Label();
			this.flashWindowTextBox = new System.Windows.Forms.TextBox();
			this.incomingMessageFlashWindowRadioButton = new System.Windows.Forms.RadioButton();
			this.incomingMessageDoNothingRadioButton = new System.Windows.Forms.RadioButton();
			this.friendOnlineGroupBox = new System.Windows.Forms.GroupBox();
			this.incomingMessagePlaySoundCheckBox = new System.Windows.Forms.CheckBox();
			this.incomingMessageSoundPathTextBox = new System.Windows.Forms.TextBox();
			this.incomingMessageSoundBrowseButton = new System.Windows.Forms.Button();
			this.friendOnlineDoNothingRadioButton = new System.Windows.Forms.RadioButton();
			this.friendOnlineMsnPopupRadioButton = new System.Windows.Forms.RadioButton();
			this.friendOnlineBalloonTooltipRadioButton = new System.Windows.Forms.RadioButton();
			this.friendOnlinePlaySoundCheckBox = new System.Windows.Forms.CheckBox();
			this.friendOnlineSoundBrowseButton = new System.Windows.Forms.Button();
			this.friendOnlineSoundPathTextBox = new System.Windows.Forms.TextBox();
			this.incomingMessageGroupBox.SuspendLayout();
			this.friendOnlineGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// incomingMessageGroupBox
			// 
			this.incomingMessageGroupBox.Controls.AddRange(new System.Windows.Forms.Control[] {
																																													this.incomingMessageSoundBrowseButton,
																																													this.incomingMessageSoundPathTextBox,
																																													this.incomingMessagePlaySoundCheckBox,
																																													this.flashWindowLabel,
																																													this.flashWindowTextBox,
																																													this.incomingMessageFlashWindowRadioButton,
																																													this.incomingMessageDoNothingRadioButton});
			this.incomingMessageGroupBox.Location = new System.Drawing.Point(8, 176);
			this.incomingMessageGroupBox.Name = "incomingMessageGroupBox";
			this.incomingMessageGroupBox.Size = new System.Drawing.Size(384, 136);
			this.incomingMessageGroupBox.TabIndex = 1;
			this.incomingMessageGroupBox.TabStop = false;
			this.incomingMessageGroupBox.Text = "Incoming message";
			// 
			// flashWindowLabel
			// 
			this.flashWindowLabel.Location = new System.Drawing.Point(176, 48);
			this.flashWindowLabel.Name = "flashWindowLabel";
			this.flashWindowLabel.Size = new System.Drawing.Size(120, 16);
			this.flashWindowLabel.TabIndex = 7;
			this.flashWindowLabel.Text = "times (zero for infinite)";
			// 
			// flashWindowTextBox
			// 
			this.flashWindowTextBox.Location = new System.Drawing.Point(113, 46);
			this.flashWindowTextBox.Name = "flashWindowTextBox";
			this.flashWindowTextBox.Size = new System.Drawing.Size(56, 20);
			this.flashWindowTextBox.TabIndex = 6;
			this.flashWindowTextBox.Text = "";
			// 
			// incomingMessageFlashWindowRadioButton
			// 
			this.incomingMessageFlashWindowRadioButton.Location = new System.Drawing.Point(16, 48);
			this.incomingMessageFlashWindowRadioButton.Name = "incomingMessageFlashWindowRadioButton";
			this.incomingMessageFlashWindowRadioButton.Size = new System.Drawing.Size(96, 16);
			this.incomingMessageFlashWindowRadioButton.TabIndex = 5;
			this.incomingMessageFlashWindowRadioButton.Text = "Flash window";
			// 
			// incomingMessageDoNothingRadioButton
			// 
			this.incomingMessageDoNothingRadioButton.Location = new System.Drawing.Point(16, 24);
			this.incomingMessageDoNothingRadioButton.Name = "incomingMessageDoNothingRadioButton";
			this.incomingMessageDoNothingRadioButton.Size = new System.Drawing.Size(352, 16);
			this.incomingMessageDoNothingRadioButton.TabIndex = 4;
			this.incomingMessageDoNothingRadioButton.Text = "Do nothing";
			// 
			// friendOnlineGroupBox
			// 
			this.friendOnlineGroupBox.Controls.AddRange(new System.Windows.Forms.Control[] {
																																											 this.friendOnlineSoundBrowseButton,
																																											 this.friendOnlineSoundPathTextBox,
																																											 this.friendOnlinePlaySoundCheckBox,
																																											 this.friendOnlineBalloonTooltipRadioButton,
																																											 this.friendOnlineMsnPopupRadioButton,
																																											 this.friendOnlineDoNothingRadioButton});
			this.friendOnlineGroupBox.Location = new System.Drawing.Point(8, 8);
			this.friendOnlineGroupBox.Name = "friendOnlineGroupBox";
			this.friendOnlineGroupBox.Size = new System.Drawing.Size(384, 152);
			this.friendOnlineGroupBox.TabIndex = 0;
			this.friendOnlineGroupBox.TabStop = false;
			this.friendOnlineGroupBox.Text = "Friend online";
			// 
			// incomingMessagePlaySoundCheckBox
			// 
			this.incomingMessagePlaySoundCheckBox.Location = new System.Drawing.Point(16, 80);
			this.incomingMessagePlaySoundCheckBox.Name = "incomingMessagePlaySoundCheckBox";
			this.incomingMessagePlaySoundCheckBox.Size = new System.Drawing.Size(350, 16);
			this.incomingMessagePlaySoundCheckBox.TabIndex = 8;
			this.incomingMessagePlaySoundCheckBox.Text = "Play sound";
			// 
			// incomingMessageSoundPathTextBox
			// 
			this.incomingMessageSoundPathTextBox.Location = new System.Drawing.Point(32, 104);
			this.incomingMessageSoundPathTextBox.Name = "incomingMessageSoundPathTextBox";
			this.incomingMessageSoundPathTextBox.Size = new System.Drawing.Size(272, 20);
			this.incomingMessageSoundPathTextBox.TabIndex = 9;
			this.incomingMessageSoundPathTextBox.Text = "";
			// 
			// incomingMessageSoundBrowseButton
			// 
			this.incomingMessageSoundBrowseButton.Location = new System.Drawing.Point(312, 104);
			this.incomingMessageSoundBrowseButton.Name = "incomingMessageSoundBrowseButton";
			this.incomingMessageSoundBrowseButton.Size = new System.Drawing.Size(64, 24);
			this.incomingMessageSoundBrowseButton.TabIndex = 10;
			this.incomingMessageSoundBrowseButton.Text = "Browse...";
			this.incomingMessageSoundBrowseButton.Click += new System.EventHandler(this.incomingMessageSoundBrowseButton_Click);
			// 
			// friendOnlineDoNothingRadioButton
			// 
			this.friendOnlineDoNothingRadioButton.Location = new System.Drawing.Point(16, 24);
			this.friendOnlineDoNothingRadioButton.Name = "friendOnlineDoNothingRadioButton";
			this.friendOnlineDoNothingRadioButton.Size = new System.Drawing.Size(352, 16);
			this.friendOnlineDoNothingRadioButton.TabIndex = 0;
			this.friendOnlineDoNothingRadioButton.Text = "Do nothing";
			// 
			// friendOnlineMsnPopupRadioButton
			// 
			this.friendOnlineMsnPopupRadioButton.Location = new System.Drawing.Point(16, 48);
			this.friendOnlineMsnPopupRadioButton.Name = "friendOnlineMsnPopupRadioButton";
			this.friendOnlineMsnPopupRadioButton.Size = new System.Drawing.Size(352, 16);
			this.friendOnlineMsnPopupRadioButton.TabIndex = 1;
			this.friendOnlineMsnPopupRadioButton.Text = "\"MSN style\" pop up window";
			// 
			// friendOnlineBalloonTooltipRadioButton
			// 
			this.friendOnlineBalloonTooltipRadioButton.Location = new System.Drawing.Point(16, 72);
			this.friendOnlineBalloonTooltipRadioButton.Name = "friendOnlineBalloonTooltipRadioButton";
			this.friendOnlineBalloonTooltipRadioButton.Size = new System.Drawing.Size(352, 16);
			this.friendOnlineBalloonTooltipRadioButton.TabIndex = 2;
			this.friendOnlineBalloonTooltipRadioButton.Text = "Balloon tooltip (requires system tray icon)";
			// 
			// friendOnlinePlaySoundCheckBox
			// 
			this.friendOnlinePlaySoundCheckBox.Location = new System.Drawing.Point(16, 96);
			this.friendOnlinePlaySoundCheckBox.Name = "friendOnlinePlaySoundCheckBox";
			this.friendOnlinePlaySoundCheckBox.Size = new System.Drawing.Size(350, 16);
			this.friendOnlinePlaySoundCheckBox.TabIndex = 9;
			this.friendOnlinePlaySoundCheckBox.Text = "Play sound";
			// 
			// friendOnlineSoundBrowseButton
			// 
			this.friendOnlineSoundBrowseButton.Location = new System.Drawing.Point(312, 120);
			this.friendOnlineSoundBrowseButton.Name = "friendOnlineSoundBrowseButton";
			this.friendOnlineSoundBrowseButton.Size = new System.Drawing.Size(64, 24);
			this.friendOnlineSoundBrowseButton.TabIndex = 12;
			this.friendOnlineSoundBrowseButton.Text = "Browse...";
			this.friendOnlineSoundBrowseButton.Click += new System.EventHandler(this.friendOnlineSoundBrowseButton_Click);
			// 
			// friendOnlineSoundPathTextBox
			// 
			this.friendOnlineSoundPathTextBox.Location = new System.Drawing.Point(32, 120);
			this.friendOnlineSoundPathTextBox.Name = "friendOnlineSoundPathTextBox";
			this.friendOnlineSoundPathTextBox.Size = new System.Drawing.Size(272, 20);
			this.friendOnlineSoundPathTextBox.TabIndex = 11;
			this.friendOnlineSoundPathTextBox.Text = "";
			// 
			// EventOptions
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.incomingMessageGroupBox,
																																	this.friendOnlineGroupBox});
			this.Name = "EventOptions";
			this.Size = new System.Drawing.Size(400, 320);
			this.incomingMessageGroupBox.ResumeLayout(false);
			this.friendOnlineGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void friendOnlineSoundBrowseButton_Click(object sender, System.EventArgs e)
		{
			string path = this.friendOnlineSoundPathTextBox.Text;
			ShowOpenSoundDialog(ref path);
			this.friendOnlineSoundPathTextBox.Text = path;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void incomingMessageSoundBrowseButton_Click(object sender, System.EventArgs e)
		{
			string path = this.incomingMessageSoundPathTextBox.Text;
			ShowOpenSoundDialog(ref path);
			this.incomingMessageSoundPathTextBox.Text = path;
		}


		/// <summary>
		/// Shows the open sound file dialog.
		/// </summary>
		/// <param name="filename">Filename of sound file selected will be put into this parameter.</param>
		/// <returns></returns>
		private WinForms.DialogResult ShowOpenSoundDialog(ref string filename)
		{
			WinForms.OpenFileDialog fileDialog = new WinForms.OpenFileDialog();

			fileDialog.CheckFileExists = fileDialog.CheckPathExists = true;
			fileDialog.Multiselect = false;
			fileDialog.Filter = "wav files (*.wav)|*.wav|All files (*.*)|*.*";

			WinForms.DialogResult dr = fileDialog.ShowDialog();

			if (dr == DialogResult.OK)
				filename = fileDialog.FileName;

			return dr;
		}

	}
}
