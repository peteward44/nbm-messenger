using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using NBM.Plugin;
using NBM.Plugin.Settings;

// 27/6/03

namespace NBM.OptionNodes
{
	/// <summary>
	/// Conversation options - options for conversations. (emoticons and other crap)
	/// </summary>
	public class ConversationOptions : System.Windows.Forms.UserControl, IOptions
	{
		private GlobalSettings settings = GlobalSettings.Instance();

		private System.Windows.Forms.CheckBox typingNotifyCheckBox;
		private System.Windows.Forms.GroupBox timeStampGroupBox;
		private System.Windows.Forms.CheckBox timeStampCheckBox;
		private System.Windows.Forms.Label variablesLabel;
		private System.Windows.Forms.Label variableListLabel1;
		private System.Windows.Forms.Label variableListLabel2;
		private System.Windows.Forms.Label formatLabel;
		private System.Windows.Forms.TextBox timeStampFormatTextBox;
		private System.Windows.Forms.CheckBox showEmoticonsCheckBox;
		private System.Windows.Forms.Label variableListLabel3;


		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary>
		/// Constructs a ConversationOptions
		/// </summary>
		public ConversationOptions()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
			this.variableListLabel1.Text = "%H - 24 hour\n%h - 12 hour\n%m -  Minute\n%s - Second";
			this.variableListLabel2.Text = "%D - Day number\n%d - Day name\n%M - Month number\n%N - Month name";
			this.variableListLabel3.Text = "%E - Week day number\n%Y - Year number";

			this.typingNotifyCheckBox.Checked = this.settings.SendTypingNotifications;
			this.showEmoticonsCheckBox.Checked = this.settings.ShowEmoticons;
			this.timeStampCheckBox.Checked = this.settings.TimeStampConversations;
			this.timeStampFormatTextBox.Text = this.settings.TimeStampFormat;
		}


		/// <summary>
		/// Name of node
		/// </summary>
		public string NodeName
		{
			get { return "Conversations"; }
		}


		/// <summary>
		/// Save settings
		/// </summary>
		public void Save()
		{
			this.settings.SendTypingNotifications = this.typingNotifyCheckBox.Checked;
			this.settings.ShowEmoticons = this.showEmoticonsCheckBox.Checked;
			this.settings.TimeStampConversations = this.timeStampCheckBox.Checked;
			this.settings.TimeStampFormat = this.timeStampFormatTextBox.Text;
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.typingNotifyCheckBox = new System.Windows.Forms.CheckBox();
			this.timeStampGroupBox = new System.Windows.Forms.GroupBox();
			this.variableListLabel3 = new System.Windows.Forms.Label();
			this.timeStampFormatTextBox = new System.Windows.Forms.TextBox();
			this.formatLabel = new System.Windows.Forms.Label();
			this.variableListLabel2 = new System.Windows.Forms.Label();
			this.variableListLabel1 = new System.Windows.Forms.Label();
			this.variablesLabel = new System.Windows.Forms.Label();
			this.timeStampCheckBox = new System.Windows.Forms.CheckBox();
			this.showEmoticonsCheckBox = new System.Windows.Forms.CheckBox();
			this.timeStampGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// typingNotifyCheckBox
			// 
			this.typingNotifyCheckBox.Location = new System.Drawing.Point(16, 16);
			this.typingNotifyCheckBox.Name = "typingNotifyCheckBox";
			this.typingNotifyCheckBox.Size = new System.Drawing.Size(360, 16);
			this.typingNotifyCheckBox.TabIndex = 0;
			this.typingNotifyCheckBox.Text = "Send typing notifications";
			// 
			// timeStampGroupBox
			// 
			this.timeStampGroupBox.Controls.AddRange(new System.Windows.Forms.Control[] {
																																										this.variableListLabel3,
																																										this.timeStampFormatTextBox,
																																										this.formatLabel,
																																										this.variableListLabel2,
																																										this.variableListLabel1,
																																										this.variablesLabel,
																																										this.timeStampCheckBox});
			this.timeStampGroupBox.Location = new System.Drawing.Point(8, 72);
			this.timeStampGroupBox.Name = "timeStampGroupBox";
			this.timeStampGroupBox.Size = new System.Drawing.Size(384, 136);
			this.timeStampGroupBox.TabIndex = 2;
			this.timeStampGroupBox.TabStop = false;
			// 
			// variableListLabel3
			// 
			this.variableListLabel3.Location = new System.Drawing.Point(248, 44);
			this.variableListLabel3.Name = "variableListLabel3";
			this.variableListLabel3.Size = new System.Drawing.Size(128, 48);
			this.variableListLabel3.TabIndex = 8;
			// 
			// timeStampFormatTextBox
			// 
			this.timeStampFormatTextBox.Location = new System.Drawing.Point(72, 104);
			this.timeStampFormatTextBox.Name = "timeStampFormatTextBox";
			this.timeStampFormatTextBox.Size = new System.Drawing.Size(96, 20);
			this.timeStampFormatTextBox.TabIndex = 7;
			this.timeStampFormatTextBox.Text = "";
			// 
			// formatLabel
			// 
			this.formatLabel.Location = new System.Drawing.Point(16, 104);
			this.formatLabel.Name = "formatLabel";
			this.formatLabel.Size = new System.Drawing.Size(48, 16);
			this.formatLabel.TabIndex = 6;
			this.formatLabel.Text = "Format:";
			this.formatLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// variableListLabel2
			// 
			this.variableListLabel2.Location = new System.Drawing.Point(128, 48);
			this.variableListLabel2.Name = "variableListLabel2";
			this.variableListLabel2.Size = new System.Drawing.Size(112, 48);
			this.variableListLabel2.TabIndex = 5;
			// 
			// variableListLabel1
			// 
			this.variableListLabel1.Location = new System.Drawing.Point(24, 48);
			this.variableListLabel1.Name = "variableListLabel1";
			this.variableListLabel1.Size = new System.Drawing.Size(96, 48);
			this.variableListLabel1.TabIndex = 4;
			// 
			// variablesLabel
			// 
			this.variablesLabel.Location = new System.Drawing.Point(16, 24);
			this.variablesLabel.Name = "variablesLabel";
			this.variablesLabel.Size = new System.Drawing.Size(56, 16);
			this.variablesLabel.TabIndex = 3;
			this.variablesLabel.Text = "Variables:";
			// 
			// timeStampCheckBox
			// 
			this.timeStampCheckBox.Location = new System.Drawing.Point(8, 0);
			this.timeStampCheckBox.Name = "timeStampCheckBox";
			this.timeStampCheckBox.Size = new System.Drawing.Size(160, 16);
			this.timeStampCheckBox.TabIndex = 2;
			this.timeStampCheckBox.Text = "Time stamp conversations";
			this.timeStampCheckBox.CheckedChanged += new System.EventHandler(this.timeStampCheckBox_CheckedChanged);
			// 
			// showEmoticonsCheckBox
			// 
			this.showEmoticonsCheckBox.Location = new System.Drawing.Point(16, 40);
			this.showEmoticonsCheckBox.Name = "showEmoticonsCheckBox";
			this.showEmoticonsCheckBox.Size = new System.Drawing.Size(360, 16);
			this.showEmoticonsCheckBox.TabIndex = 3;
			this.showEmoticonsCheckBox.Text = "Show emoticons (smilies etc)";
			// 
			// ConversationOptions
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.showEmoticonsCheckBox,
																																	this.timeStampGroupBox,
																																	this.typingNotifyCheckBox});
			this.Name = "ConversationOptions";
			this.Size = new System.Drawing.Size(400, 320);
			this.timeStampGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timeStampCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			this.timeStampFormatTextBox.Enabled = this.timeStampCheckBox.Checked;
		}
	}
}
