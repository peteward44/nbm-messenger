using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using WinForms = System.Windows.Forms;

// 10/7/03

namespace NBM
{
	/// <summary>
	/// Form that pops up whenever you want to add a fried to your contact list.
	/// </summary>
	public class AddFriendForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button cancelButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox usernameBox;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox messageTextBox;

		private Protocol protocol;


		/// <summary>
		/// Username of user to add
		/// </summary>
		public string Username
		{
			get { return this.usernameBox.Text; }
		}


		/// <summary>
		/// If the protocol supports messages when adding users, this is the message.
		/// </summary>
		public string Message
		{
			get { return this.messageTextBox.Text; }
		}


		/// <summary>
		/// Constructs an AddFriendForm.
		/// </summary>
		/// <param name="protocol">Associated protocol.</param>
		public AddFriendForm(Protocol protocol)
		{
			this.protocol = protocol;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.messageTextBox.Enabled = protocol.Settings.Constants.ReasonSentWithAddFriend;
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
			this.addButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.usernameBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.messageTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// addButton
			// 
			this.addButton.Location = new System.Drawing.Point(88, 120);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(80, 24);
			this.addButton.TabIndex = 1;
			this.addButton.Text = "Add";
			this.addButton.Click += new System.EventHandler(this.addButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(184, 120);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(80, 24);
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "Username:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// usernameBox
			// 
			this.usernameBox.Location = new System.Drawing.Point(80, 16);
			this.usernameBox.Name = "usernameBox";
			this.usernameBox.Size = new System.Drawing.Size(184, 20);
			this.usernameBox.TabIndex = 0;
			this.usernameBox.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "Message:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// messageTextBox
			// 
			this.messageTextBox.Location = new System.Drawing.Point(80, 48);
			this.messageTextBox.Multiline = true;
			this.messageTextBox.Name = "messageTextBox";
			this.messageTextBox.Size = new System.Drawing.Size(184, 56);
			this.messageTextBox.TabIndex = 4;
			this.messageTextBox.Text = "";
			// 
			// AddFriendForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(274, 152);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.messageTextBox,
																																	this.label2,
																																	this.usernameBox,
																																	this.label1,
																																	this.cancelButton,
																																	this.addButton});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "AddFriendForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Add Friend";
			this.ResumeLayout(false);

		}
		#endregion


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void addButton_Click(object sender, System.EventArgs e)
		{
			if (this.usernameBox.Text == "" || this.usernameBox.Text == string.Empty)
			{
				WinForms.MessageBox.Show(this, "Username cannot be empty");
			}
			else
			{
				this.DialogResult = WinForms.DialogResult.OK;
				this.Close();
			}
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
