using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using NBM.Plugin;

// 1/7/03

namespace NBM
{
	/// <summary>
	/// When a some stranger adds you to their contact list, this form pops up.
	/// </summary>
	public class OnFriendAddForm : System.Windows.Forms.Form
	{
		private IProtocol protocol;
		private Friend friend;

		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.CheckBox addUserCheckBox;
		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.RadioButton allowUserRadioButton;
		private System.Windows.Forms.RadioButton dontAllowUserRadioButton;


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary>
		/// Constructs an OnFriendAddForm form.
		/// </summary>
		/// <param name="protocol">Associated protocol</param>
		/// <param name="friend">Person who's added you to their list</param>
		/// <param name="reason">[optional] Stranger's message</param>
		/// <param name="enableAddCheckbox">Whether to enable the add user checkbox</param>
		public OnFriendAddForm(IProtocol protocol, Friend friend, string reason, bool enableAddCheckbox)
		{
			this.protocol = protocol;
			this.friend = friend;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.label.Text = friend.DisplayName + " (" + friend.Username + ") "
				+ "has added you to their contact list";
			this.textBox.Text = reason;
			this.addUserCheckBox.Enabled = enableAddCheckbox;

			this.allowUserRadioButton.Checked = true;
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
			this.label = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.addUserCheckBox = new System.Windows.Forms.CheckBox();
			this.allowUserRadioButton = new System.Windows.Forms.RadioButton();
			this.dontAllowUserRadioButton = new System.Windows.Forms.RadioButton();
			this.textBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label
			// 
			this.label.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.label.Location = new System.Drawing.Point(8, 8);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(250, 40);
			this.label.TabIndex = 0;
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(136, 208);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(80, 24);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// okButton
			// 
			this.okButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(40, 208);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(80, 24);
			this.okButton.TabIndex = 2;
			this.okButton.Text = "OK";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// addUserCheckBox
			// 
			this.addUserCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.addUserCheckBox.Location = new System.Drawing.Point(8, 176);
			this.addUserCheckBox.Name = "addUserCheckBox";
			this.addUserCheckBox.Size = new System.Drawing.Size(248, 16);
			this.addUserCheckBox.TabIndex = 3;
			this.addUserCheckBox.Text = "Add this user to my contacts list";
			// 
			// allowUserRadioButton
			// 
			this.allowUserRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.allowUserRadioButton.Location = new System.Drawing.Point(8, 120);
			this.allowUserRadioButton.Name = "allowUserRadioButton";
			this.allowUserRadioButton.Size = new System.Drawing.Size(248, 16);
			this.allowUserRadioButton.TabIndex = 4;
			this.allowUserRadioButton.Text = "Allow user to see my online status";
			// 
			// dontAllowUserRadioButton
			// 
			this.dontAllowUserRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.dontAllowUserRadioButton.Location = new System.Drawing.Point(8, 144);
			this.dontAllowUserRadioButton.Name = "dontAllowUserRadioButton";
			this.dontAllowUserRadioButton.Size = new System.Drawing.Size(248, 16);
			this.dontAllowUserRadioButton.TabIndex = 5;
			this.dontAllowUserRadioButton.Text = "Dont allow this user to see my online status";
			// 
			// textBox
			// 
			this.textBox.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.textBox.Location = new System.Drawing.Point(8, 56);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.ReadOnly = true;
			this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textBox.Size = new System.Drawing.Size(248, 56);
			this.textBox.TabIndex = 6;
			this.textBox.Text = "";
			// 
			// OnFriendAddForm
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(266, 240);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.textBox,
																																	this.dontAllowUserRadioButton,
																																	this.allowUserRadioButton,
																																	this.addUserCheckBox,
																																	this.okButton,
																																	this.cancelButton,
																																	this.label});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OnFriendAddForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.ResumeLayout(false);

		}
		#endregion


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void okButton_Click(object sender, System.EventArgs e)
		{
			this.protocol.PromptForStrangerHasAddedMeResponse(friend, "",
				this.allowUserRadioButton.Checked, this.addUserCheckBox.Checked);
			this.Close();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
