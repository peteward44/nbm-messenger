using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using WinForms = System.Windows.Forms;
using IO = System.IO;

// 28/6/03

namespace NBM.Diagnostics
{
	/// <summary>
	/// Debug information window
	/// Accessible through the protocol menu -> Debug Report
	/// </summary>
	public class Debug : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.Button copyToClipboardButton;
		private System.Windows.Forms.TextBox outputTextBox;


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="postfix">String to append to the end of the window text</param>
		public Debug(string postfix)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.Text += " : " + postfix;
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
		/// Overrided WndProc so the window is hidden when the X button is clicked
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref WinForms.Message m)
		{
			switch (m.Msg)
			{
				case 0x0010: // WM_CLOSE
					this.Hide();
					return;
			}

			base.WndProc(ref m);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			NativeMethods.VerticalScrollToBottom(this.outputTextBox);
			base.OnActivated(e);
		}


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.outputTextBox = new System.Windows.Forms.TextBox();
			this.closeButton = new System.Windows.Forms.Button();
			this.copyToClipboardButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// outputTextBox
			// 
			this.outputTextBox.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.outputTextBox.Location = new System.Drawing.Point(8, 8);
			this.outputTextBox.Multiline = true;
			this.outputTextBox.Name = "outputTextBox";
			this.outputTextBox.ReadOnly = true;
			this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.outputTextBox.Size = new System.Drawing.Size(520, 272);
			this.outputTextBox.TabIndex = 0;
			this.outputTextBox.Text = "";
			// 
			// closeButton
			// 
			this.closeButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.closeButton.Location = new System.Drawing.Point(456, 296);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(72, 32);
			this.closeButton.TabIndex = 1;
			this.closeButton.Text = "Hide";
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// copyToClipboardButton
			// 
			this.copyToClipboardButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.copyToClipboardButton.Location = new System.Drawing.Point(312, 296);
			this.copyToClipboardButton.Name = "copyToClipboardButton";
			this.copyToClipboardButton.Size = new System.Drawing.Size(120, 32);
			this.copyToClipboardButton.TabIndex = 2;
			this.copyToClipboardButton.Text = "Copy To Clipboard";
			this.copyToClipboardButton.Click += new System.EventHandler(this.copyToClipboardButton_Click);
			// 
			// Debug
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(536, 334);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.copyToClipboardButton,
																																	this.closeButton,
																																	this.outputTextBox});
			this.Name = "Debug";
			this.Text = "Debug Output";
			this.ResumeLayout(false);

		}
		#endregion


		/// <summary>
		/// Copies the debug information to the clipboard.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void copyToClipboardButton_Click(object sender, System.EventArgs e)
		{
			WinForms.Clipboard.SetDataObject(this.outputTextBox.Text, true);
		}


		/// <summary>
		/// Hides the window.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void closeButton_Click(object sender, System.EventArgs e)
		{
			this.Hide();
		}


		#region Write methods



		/// <summary>
		/// Writes a line to the debug window.
		/// </summary>
		/// <param name="text"></param>
		public void WriteLine(string text)
		{
			this.outputTextBox.Text += text + "\r\n";
			NativeMethods.VerticalScrollToBottom(this.outputTextBox);
		}


		/// <summary>
		/// Writes a line to the debug window.
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void WriteLine(string format, params object[] args)
		{
			WriteLine(string.Format(format, args));
		}


		#endregion

	}
}
