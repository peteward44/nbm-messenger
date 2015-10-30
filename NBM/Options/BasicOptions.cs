using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using NBM.Plugin;
using NBM.Plugin.Settings;

// 26/6/03

namespace NBM.OptionNodes
{
	/// <summary>
	/// Provides basic options
	/// </summary>
	public class BasicOptions : System.Windows.Forms.UserControl, IOptions
	{
		private GlobalSettings settings = GlobalSettings.Instance();

		private System.Windows.Forms.GroupBox startupGroupBox;
		private System.Windows.Forms.GroupBox displayGroupBox;
		private System.Windows.Forms.GroupBox miscGroupBox;
		private System.Windows.Forms.CheckBox loadOnStartupCheckBox;
		private System.Windows.Forms.CheckBox connectAllCheckBox;
		private System.Windows.Forms.CheckBox hideOfflineCheckBox;
		private System.Windows.Forms.CheckBox showInTaskbarCheckBox;
		private System.Windows.Forms.CheckBox contactListOnTopCheckBox;
		private System.Windows.Forms.CheckBox promptOnExitCheckBox;
		private System.Windows.Forms.RadioButton exitXRadioButton;
		private System.Windows.Forms.RadioButton minimizeXRadioButton;


		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary>
		/// Constructs a BasicOptions
		/// </summary>
		public BasicOptions()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
			this.contactListOnTopCheckBox.Checked = this.settings.AlwaysOnTop;
			this.connectAllCheckBox.Checked = this.settings.ConnectAllOnStartup;
			this.showInTaskbarCheckBox.Checked = this.settings.DisplayTaskBar;
			this.hideOfflineCheckBox.Checked = this.settings.HideOfflineContacts;
			this.loadOnStartupCheckBox.Checked = this.settings.LoadOnStartup;
			this.promptOnExitCheckBox.Checked = this.settings.PromptOnExit;

			this.exitXRadioButton.Checked = this.settings.ExitOnX;
			this.minimizeXRadioButton.Checked = !this.settings.ExitOnX;
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
		/// Saves the options into memory
		/// </summary>
		public void Save()
		{
			this.settings.AlwaysOnTop = this.contactListOnTopCheckBox.Checked;
			this.settings.ConnectAllOnStartup = this.connectAllCheckBox.Checked;
			this.settings.DisplayTaskBar = this.showInTaskbarCheckBox.Checked;
			this.settings.HideOfflineContacts = this.hideOfflineCheckBox.Checked;
			this.settings.LoadOnStartup = this.loadOnStartupCheckBox.Checked;
			this.settings.PromptOnExit = this.promptOnExitCheckBox.Checked;
			this.settings.ExitOnX = this.exitXRadioButton.Checked;
		}


		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.startupGroupBox = new System.Windows.Forms.GroupBox();
			this.connectAllCheckBox = new System.Windows.Forms.CheckBox();
			this.loadOnStartupCheckBox = new System.Windows.Forms.CheckBox();
			this.displayGroupBox = new System.Windows.Forms.GroupBox();
			this.showInTaskbarCheckBox = new System.Windows.Forms.CheckBox();
			this.hideOfflineCheckBox = new System.Windows.Forms.CheckBox();
			this.miscGroupBox = new System.Windows.Forms.GroupBox();
			this.promptOnExitCheckBox = new System.Windows.Forms.CheckBox();
			this.contactListOnTopCheckBox = new System.Windows.Forms.CheckBox();
			this.exitXRadioButton = new System.Windows.Forms.RadioButton();
			this.minimizeXRadioButton = new System.Windows.Forms.RadioButton();
			this.startupGroupBox.SuspendLayout();
			this.displayGroupBox.SuspendLayout();
			this.miscGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// startupGroupBox
			// 
			this.startupGroupBox.Controls.AddRange(new System.Windows.Forms.Control[] {
																																									this.connectAllCheckBox,
																																									this.loadOnStartupCheckBox});
			this.startupGroupBox.Location = new System.Drawing.Point(8, 8);
			this.startupGroupBox.Name = "startupGroupBox";
			this.startupGroupBox.Size = new System.Drawing.Size(384, 80);
			this.startupGroupBox.TabIndex = 0;
			this.startupGroupBox.TabStop = false;
			this.startupGroupBox.Text = "Startup";
			// 
			// connectAllCheckBox
			// 
			this.connectAllCheckBox.Location = new System.Drawing.Point(16, 48);
			this.connectAllCheckBox.Name = "connectAllCheckBox";
			this.connectAllCheckBox.Size = new System.Drawing.Size(352, 16);
			this.connectAllCheckBox.TabIndex = 1;
			this.connectAllCheckBox.Text = "Connect all protocols on startup";
			// 
			// loadOnStartupCheckBox
			// 
			this.loadOnStartupCheckBox.Location = new System.Drawing.Point(16, 24);
			this.loadOnStartupCheckBox.Name = "loadOnStartupCheckBox";
			this.loadOnStartupCheckBox.Size = new System.Drawing.Size(352, 16);
			this.loadOnStartupCheckBox.TabIndex = 0;
			this.loadOnStartupCheckBox.Text = "Load on startup";
			// 
			// displayGroupBox
			// 
			this.displayGroupBox.Controls.AddRange(new System.Windows.Forms.Control[] {
																																									this.showInTaskbarCheckBox,
																																									this.hideOfflineCheckBox});
			this.displayGroupBox.Location = new System.Drawing.Point(8, 96);
			this.displayGroupBox.Name = "displayGroupBox";
			this.displayGroupBox.Size = new System.Drawing.Size(384, 80);
			this.displayGroupBox.TabIndex = 1;
			this.displayGroupBox.TabStop = false;
			this.displayGroupBox.Text = "Display";
			// 
			// showInTaskbarCheckBox
			// 
			this.showInTaskbarCheckBox.Location = new System.Drawing.Point(16, 48);
			this.showInTaskbarCheckBox.Name = "showInTaskbarCheckBox";
			this.showInTaskbarCheckBox.Size = new System.Drawing.Size(352, 16);
			this.showInTaskbarCheckBox.TabIndex = 1;
			this.showInTaskbarCheckBox.Text = "Show in taskbar";
			// 
			// hideOfflineCheckBox
			// 
			this.hideOfflineCheckBox.Location = new System.Drawing.Point(16, 24);
			this.hideOfflineCheckBox.Name = "hideOfflineCheckBox";
			this.hideOfflineCheckBox.Size = new System.Drawing.Size(352, 16);
			this.hideOfflineCheckBox.TabIndex = 0;
			this.hideOfflineCheckBox.Text = "Hide offline contacts";
			// 
			// miscGroupBox
			// 
			this.miscGroupBox.Controls.AddRange(new System.Windows.Forms.Control[] {
																																							 this.minimizeXRadioButton,
																																							 this.exitXRadioButton,
																																							 this.promptOnExitCheckBox,
																																							 this.contactListOnTopCheckBox});
			this.miscGroupBox.Location = new System.Drawing.Point(8, 184);
			this.miscGroupBox.Name = "miscGroupBox";
			this.miscGroupBox.Size = new System.Drawing.Size(384, 128);
			this.miscGroupBox.TabIndex = 2;
			this.miscGroupBox.TabStop = false;
			this.miscGroupBox.Text = "Miscellaneous";
			// 
			// promptOnExitCheckBox
			// 
			this.promptOnExitCheckBox.Location = new System.Drawing.Point(16, 48);
			this.promptOnExitCheckBox.Name = "promptOnExitCheckBox";
			this.promptOnExitCheckBox.Size = new System.Drawing.Size(352, 16);
			this.promptOnExitCheckBox.TabIndex = 1;
			this.promptOnExitCheckBox.Text = "Prompt on exit";
			// 
			// contactListOnTopCheckBox
			// 
			this.contactListOnTopCheckBox.Location = new System.Drawing.Point(16, 24);
			this.contactListOnTopCheckBox.Name = "contactListOnTopCheckBox";
			this.contactListOnTopCheckBox.Size = new System.Drawing.Size(352, 16);
			this.contactListOnTopCheckBox.TabIndex = 0;
			this.contactListOnTopCheckBox.Text = "Contact list always on top";
			// 
			// exitXRadioButton
			// 
			this.exitXRadioButton.Location = new System.Drawing.Point(16, 80);
			this.exitXRadioButton.Name = "exitXRadioButton";
			this.exitXRadioButton.Size = new System.Drawing.Size(352, 16);
			this.exitXRadioButton.TabIndex = 2;
			this.exitXRadioButton.Text = "Exit NBM when \"X\" button is clicked";
			// 
			// minimizeXRadioButton
			// 
			this.minimizeXRadioButton.Location = new System.Drawing.Point(16, 104);
			this.minimizeXRadioButton.Name = "minimizeXRadioButton";
			this.minimizeXRadioButton.Size = new System.Drawing.Size(352, 16);
			this.minimizeXRadioButton.TabIndex = 3;
			this.minimizeXRadioButton.Text = "Minimize to system tray when \"X\" button is clicked";
			// 
			// BasicOptions
			// 
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.miscGroupBox,
																																	this.displayGroupBox,
																																	this.startupGroupBox});
			this.Name = "BasicOptions";
			this.Size = new System.Drawing.Size(400, 320);
			this.startupGroupBox.ResumeLayout(false);
			this.displayGroupBox.ResumeLayout(false);
			this.miscGroupBox.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
