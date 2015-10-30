using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using WinForms = System.Windows.Forms;

// 13/6/03

namespace NBM
{
	/// <summary>
	/// When the program crashes, this form pops up.
	/// </summary>
	public class BugReportForm : WinForms.Form
	{
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.Button submitBugReportButton;
		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Button copyToClipboardButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary>
		/// Constructs a bug report form.
		/// </summary>
		public BugReportForm()
			: this(null)
		{
		}


		/// <summary>
		/// Constructs a bug report form.
		/// </summary>
		/// <param name="e"></param>
		public BugReportForm(Exception e)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			if (e != null)
			{
				this.textBox.Text = "Message: " + e.Message + "\r\n\r\nStack Trace:\r\n\r\n" + e.StackTrace;
			}
			else
			{
				this.textBox.Text = "No information available - unknown exception caught";
				this.submitBugReportButton.Enabled = this.copyToClipboardButton.Enabled = false;
			}
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
			this.closeButton = new System.Windows.Forms.Button();
			this.submitBugReportButton = new System.Windows.Forms.Button();
			this.copyToClipboardButton = new System.Windows.Forms.Button();
			this.textBox = new System.Windows.Forms.TextBox();
			this.label = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// closeButton
			// 
			this.closeButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closeButton.Location = new System.Drawing.Point(384, 312);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(80, 24);
			this.closeButton.TabIndex = 0;
			this.closeButton.Text = "Close";
			// 
			// submitBugReportButton
			// 
			this.submitBugReportButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.submitBugReportButton.Enabled = false;
			this.submitBugReportButton.Location = new System.Drawing.Point(8, 312);
			this.submitBugReportButton.Name = "submitBugReportButton";
			this.submitBugReportButton.Size = new System.Drawing.Size(112, 24);
			this.submitBugReportButton.TabIndex = 1;
			this.submitBugReportButton.Text = "Submit Bug Report";
			this.submitBugReportButton.Click += new System.EventHandler(this.submitBugReportButton_Click);
			// 
			// copyToClipboardButton
			// 
			this.copyToClipboardButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			this.copyToClipboardButton.Location = new System.Drawing.Point(128, 312);
			this.copyToClipboardButton.Name = "copyToClipboardButton";
			this.copyToClipboardButton.Size = new System.Drawing.Size(112, 24);
			this.copyToClipboardButton.TabIndex = 2;
			this.copyToClipboardButton.Text = "Copy To Clipboard";
			this.copyToClipboardButton.Click += new System.EventHandler(this.copyToClipboardButton_Click);
			// 
			// textBox
			// 
			this.textBox.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.textBox.Location = new System.Drawing.Point(8, 48);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.ReadOnly = true;
			this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox.Size = new System.Drawing.Size(464, 248);
			this.textBox.TabIndex = 3;
			this.textBox.Text = "";
			this.textBox.WordWrap = false;
			// 
			// label
			// 
			this.label.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.label.Location = new System.Drawing.Point(8, 8);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(464, 32);
			this.label.TabIndex = 4;
			this.label.Text = "Looks like NBM has crashed, sorry. Perhaps you should think about using Trillian," +
				" Miranda or one of the other thousands of IM programs out there instead.";
			// 
			// BugReportForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.closeButton;
			this.ClientSize = new System.Drawing.Size(480, 342);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.label,
																																	this.textBox,
																																	this.copyToClipboardButton,
																																	this.submitBugReportButton,
																																	this.closeButton});
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(368, 376);
			this.Name = "BugReportForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "A fatal error has occurred";
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion


		/// <summary>
		/// Copies the error information to the clipboard.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void copyToClipboardButton_Click(object sender, System.EventArgs e)
		{
			WinForms.Clipboard.SetDataObject(this.textBox.Text, true);
		}


		/// <summary>
		/// Submits the bug report information to me.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void submitBugReportButton_Click(object sender, System.EventArgs e)
		{
		}

	}
}
