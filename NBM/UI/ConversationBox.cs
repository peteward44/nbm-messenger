using System;
using NBM.Plugin;
using NBM.Plugin.Settings;

using WinForms = System.Windows.Forms;
using Drawing = System.Drawing;

// 20/5/03

namespace NBM
{
	/// <summary>
	/// Text box for conversations. (in the MessageForm)
	/// </summary>
	public class ConversationBox : WinForms.RichTextBox
	{
		private readonly Drawing.Color eventColour = Drawing.Color.DarkRed;


		/// <summary>
		/// Constructs a ConversationBox.
		/// </summary>
		public ConversationBox()
		{
			this.DetectUrls = true;
			this.LinkClicked += new WinForms.LinkClickedEventHandler(OnLinkClicked);
			this.ScrollBars = WinForms.RichTextBoxScrollBars.ForcedVertical;
			this.AutoWordSelection = true;
			this.ReadOnly = true;
		}


		/// <summary>
		/// Scrolls down the box to the bottom.
		/// </summary>
		public void ScrollToBottom()
		{
			NativeMethods.VerticalScrollToBottom(this);
		}


		/// <summary>
		/// When a link is clicked inside the box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnLinkClicked(object sender, WinForms.LinkClickedEventArgs args)
		{
			System.Diagnostics.Process.Start(args.LinkText);
		}


		private delegate void WriteLineHandler(string name, string text, Drawing.Color colour);


		/// <summary>
		/// Writes a line to the conversation box. This is Invoke()ed from WriteLine().
		/// </summary>
		/// <param name="name">Name of the person whos saying something.</param>
		/// <param name="text">Text of the message</param>
		/// <param name="colour">Colour to write name in.</param>
		private void WriteLineMethod(string name, string text, Drawing.Color colour)
		{
			string time = "";

			if (GlobalSettings.Instance().TimeStampConversations)
			{
				time = Time.GetFormattedTime() + " : ";
			}

			if (!(this.Text == string.Empty || this.Text == ""))
			{
				time = "\r\n" + time;
			}

			this.Select(this.Text.Length, 0);
			this.SelectionColor = colour;
			this.AppendText(time + name + ": ");
			this.SelectionColor = Drawing.Color.Black;
			this.AppendText(text);
			this.SelectionColor = Drawing.Color.Empty;

			this.ScrollToBottom();
		}


		/// <summary>
		/// Writes a line to the conversation box. Automatically handles timestamping and colour etc
		/// </summary>
		/// <param name="name"></param>
		/// <param name="text"></param>
		/// <param name="colour"></param>
		public void WriteLine(string name, string text, Drawing.Color colour)
		{
			this.Invoke(new WriteLineHandler(WriteLineMethod), new object[] { name, text, colour });
		}


		private delegate void WriteEventHandler(string text);


		/// <summary>
		/// Writes an event to the conversation box. This is Invoke()ed from WriteEvent().
		/// </summary>
		/// <param name="text">Description of the event</param>
		private void WriteEventMethod(string text)
		{
			if (GlobalSettings.Instance().TimeStampConversations)
			{
				text = Time.GetFormattedTime() + " " + text;
			}

			if (!(this.Text == string.Empty || this.Text == ""))
			{
				text = "\r\n" + text;
			}

			this.Select(this.Text.Length, 0);
			this.SelectionColor = this.eventColour;
			this.AppendText(text);
			this.SelectionColor = Drawing.Color.Empty;

			this.ScrollToBottom();
		}


		/// <summary>
		/// Writes an event to the conversation box. Handles timestamping and colour etc
		/// </summary>
		/// <param name="text">Description of the event.</param>
		public void WriteEvent(string text)
		{
			this.Invoke(new WriteEventHandler(WriteEventMethod), new object[] { text });
		}
	}
}
