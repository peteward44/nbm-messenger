using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;

using NBM.Plugin;
using NBM.Plugin.Settings;

using WinForms = System.Windows.Forms;
using Drawing = System.Drawing;
using IO = System.IO;

// 11/6/03

namespace NBM
{
	/// <summary>
	/// Form for conversations. This class pretty much controls everything with conversations.
	/// </summary>
	public class MessageForm : WinForms.Form, IProtocolListener, IConversationListener
	{
		private WinForms.TextBox typingInBox;
		private WinForms.Button sendButton;

		private volatile Queue textQueue = new Queue();
		private ConversationBox convoBox;
		private volatile bool connectionInProgress = true, conversationConnected = false;

		private readonly Drawing.Color friendSpeakColour = Drawing.Color.Blue;
		private readonly Drawing.Color speakColour = Drawing.Color.Red;

		private Protocol protocol;
		private Friend originalFriend;
		private IConversation conversation;

		private ConversationServer conversationServer;
		private System.Windows.Forms.StatusBar statusBar;

		private ILog log;

		private System.Timers.Timer typingNotifyTimer = new System.Timers.Timer(10 * 1000);
		private System.Windows.Forms.ToolBar toolBar;
		private System.Windows.Forms.ToolBarButton fileTransferButton;


		/// <summary>
		/// Friend who started the conversation / who we clicked on
		/// </summary>
		public Friend OriginalFriend
		{
			get { return originalFriend; }
		}


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary>
		/// Constructs a MessageForm
		/// </summary>
		/// <param name="protocol">Associated protocol</param>
		/// <param name="originalFriend">Initial friend</param>
		public MessageForm(Protocol protocol, Friend originalFriend)
		{
			this.protocol = protocol;
			this.originalFriend = originalFriend;

			this.log = LogFactory.CreateLog(protocol, originalFriend.Username);

			this.protocol.AddListener(this);

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// set main text
			this.Text = originalFriend.DisplayName + " (" + originalFriend.Username + ") " + " - " + protocol.Name;

			// setup typing notification timer
			typingNotifyTimer.AutoReset = false;
			typingNotifyTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTypingNotifyTimerElapse);

			// enable file transfer
			this.toolBar.Buttons[0].Enabled = this.protocol.Settings.Constants.SupportsFileTransfer;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClosed(EventArgs e)
		{
			// end the conversation then unhook the event handlers
			if (this.conversation != null && this.conversationConnected)
			{
				this.conversation.End(new OperationCompleteEvent());
				this.conversationServer.OnDisconnected();
			}

			this.protocol.RemoveListener(this);

			if (this.conversationServer != null)
			{
				this.conversationServer.RemoveListener(this);
				this.conversationServer.RemoveListener(this.log);
			}

			base.OnClosed(e);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			this.convoBox.ScrollToBottom();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnActivated(EventArgs e)
		{
			this.typingInBox.Focus();
			base.OnActivated(e);
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

				if (this.log != null)
				{
					this.log.Dispose();
					this.log = null;
				}
			}
			base.Dispose( disposing );
		}


		/// <summary>
		/// Connect the conversation up.
		/// </summary>
		/// <param name="byinvite">True if the conversation was initiated by someone on the contact list, false if the user clicked on the name</param>
		/// <param name="tag">If the conversation was started by someone on the contact list, this is a plugin-defined object</param>
		/// <param name="opCompleteEvent"></param>
		public void Connect(bool byinvite, object tag, OperationCompleteEvent opCompleteEvent)
		{
			this.conversationServer = new ConversationServer(this.protocol.Server);

			this.conversationServer.AddListener(this);
			this.conversationServer.AddListener(log);

			this.conversation = this.protocol.ClassFactory.CreateConversation(
				this.protocol.IProtocol,
				new ConversationControl(this.conversationServer),
				this.protocol.Settings);

			opCompleteEvent.RegisterEvent(new OperationCompleteHandler(OnConnectionComplete));

			if (byinvite)
				this.conversation.StartByInvitation(this.originalFriend, opCompleteEvent, tag);
			else
				this.conversation.Start(this.originalFriend, opCompleteEvent);

			this.connectionInProgress = true;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <param name="tag"></param>
		private void OnConnectionComplete(OperationCompleteArgs args, object tag)
		{
			if (args.Success)
			{
				// if the user typed anything in whilst we were connecting, flush it all out
				this.conversationConnected = true;
				this.FlushTextQueue();
				this.conversationServer.OnConnected();
			}
			this.connectionInProgress = false;
		}


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MessageForm));
			this.typingInBox = new System.Windows.Forms.TextBox();
			this.sendButton = new System.Windows.Forms.Button();
			this.convoBox = new NBM.ConversationBox();
			this.statusBar = new System.Windows.Forms.StatusBar();
			this.toolBar = new System.Windows.Forms.ToolBar();
			this.fileTransferButton = new System.Windows.Forms.ToolBarButton();
			this.SuspendLayout();
			// 
			// typingInBox
			// 
			this.typingInBox.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.typingInBox.Location = new System.Drawing.Point(0, 188);
			this.typingInBox.Multiline = true;
			this.typingInBox.Name = "typingInBox";
			this.typingInBox.Size = new System.Drawing.Size(316, 36);
			this.typingInBox.TabIndex = 1;
			this.typingInBox.Text = "";
			this.typingInBox.TextChanged += new System.EventHandler(this.typingInBox_TextChanged);
			// 
			// sendButton
			// 
			this.sendButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			this.sendButton.Location = new System.Drawing.Point(320, 188);
			this.sendButton.Name = "sendButton";
			this.sendButton.Size = new System.Drawing.Size(56, 36);
			this.sendButton.TabIndex = 2;
			this.sendButton.Text = "Send";
			this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
			// 
			// convoBox
			// 
			this.convoBox.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.convoBox.AutoWordSelection = true;
			this.convoBox.Location = new System.Drawing.Point(0, 4);
			this.convoBox.Name = "convoBox";
			this.convoBox.ReadOnly = true;
			this.convoBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
			this.convoBox.Size = new System.Drawing.Size(380, 156);
			this.convoBox.TabIndex = 3;
			this.convoBox.Text = "";
			// 
			// statusBar
			// 
			this.statusBar.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.statusBar.Dock = System.Windows.Forms.DockStyle.None;
			this.statusBar.Location = new System.Drawing.Point(0, 228);
			this.statusBar.Name = "statusBar";
			this.statusBar.Size = new System.Drawing.Size(380, 20);
			this.statusBar.TabIndex = 4;
			// 
			// toolBar
			// 
			this.toolBar.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.toolBar.AutoSize = false;
			this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
																																							 this.fileTransferButton});
			this.toolBar.ButtonSize = new System.Drawing.Size(16, 16);
			this.toolBar.Divider = false;
			this.toolBar.Dock = System.Windows.Forms.DockStyle.None;
			this.toolBar.DropDownArrows = true;
			this.toolBar.Location = new System.Drawing.Point(4, 160);
			this.toolBar.Name = "toolBar";
			this.toolBar.ShowToolTips = true;
			this.toolBar.Size = new System.Drawing.Size(372, 28);
			this.toolBar.TabIndex = 5;
			this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar_ButtonClick);
			// 
			// fileTransferButton
			// 
			this.fileTransferButton.Tag = "SendFile";
			this.fileTransferButton.Text = "Send File";
			this.fileTransferButton.ToolTipText = "Send File";
			// 
			// MessageForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(380, 246);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.toolBar,
																																	this.statusBar,
																																	this.convoBox,
																																	this.sendButton,
																																	this.typingInBox});
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MessageForm";
			this.Text = "Message";
			this.ResumeLayout(false);

		}
		#endregion


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void sendButton_Click(object sender, System.EventArgs e)
		{
			Say();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void typingInBox_TextChanged(object sender, System.EventArgs e)
		{
			if (this.typingInBox.Text.Length == 1
				&& this.conversationConnected
				&& GlobalSettings.Instance().SendTypingNotifications)
			{
				this.conversation.SendTypingNotification(new OperationCompleteEvent());
			}

			if (this.typingInBox.Enabled && this.typingInBox.Text.EndsWith("\r\n"))
			{
				this.typingInBox.Text = this.typingInBox.Text.Substring(0, typingInBox.Text.Length-2);
				Say();
			}
		}


		/// <summary>
		/// 
		/// </summary>
		private void Say()
		{
			string text = this.typingInBox.Text;
			this.typingInBox.Clear();

			if (text == "" || text == string.Empty)
				return; // doesnt say anything useful

			WriteToConversationBox(this.protocol.Settings.DisplayName, text, this.speakColour);

			if (!this.conversationConnected)
			{
				this.textQueue.Enqueue(text);

				if (!connectionInProgress)
				{
					this.Connect(false, null, new OperationCompleteEvent());
				}
			}
			else
			{
				this.conversation.Say(text, new OperationCompleteEvent(new OperationCompleteHandler(OnSayAcknowledged), null));
				this.conversationServer.UserSay(text);
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		/// <param name="tag"></param>
		private void OnSayAcknowledged(OperationCompleteArgs args, object tag)
		{
			if (!args.Success)
			{
				this.WriteEventToConvoBox("Message could not be sent");
			}
		}


		private delegate void FlushTextHandler();


		/// <summary>
		/// 
		/// </summary>
		private void FlushTextQueue()
		{
			if (this.Created)
			{
				this.Invoke(new FlushTextHandler(DoFlushTextQueue));
			}
		}


		/// <summary>
		/// 
		/// </summary>
		private void DoFlushTextQueue()
		{
			int count = textQueue.Count;

			for (int i=0; i<count; ++i)
			{
				string text = (string)textQueue.Dequeue();
				this.conversation.Say(text, new OperationCompleteEvent());
				this.conversationServer.UserSay(text);
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="text"></param>
		/// <param name="colour"></param>
		private void WriteToConversationBox(string name, string text, Drawing.Color colour)
		{
			if (this.Created)
			{
				this.convoBox.WriteLine(name, text, colour);
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		private void WriteEventToConvoBox(string text)
		{
			if (this.Created)
			{
				this.convoBox.WriteEvent(text);
			}
		}


		#region Protocol listener methods


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnFriendAdd(Friend friend)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnFriendRemove(Friend friend)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="status"></param>
		public void OnFriendChangeStatus(Friend friend, OnlineStatus status)
		{
			if (friend.Equals(this.originalFriend))
			{
				if (status != OnlineStatus.Offline)
					this.WriteEventToConvoBox(friend.Username + " has changed status to " + status.ToString());
				else
					this.WriteEventToConvoBox(friend.Username + " has signed off");
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="newName"></param>
		public void OnFriendChangeDisplayName(Friend friend, string newName)
		{
			if (friend.Equals(this.originalFriend))
				this.Text = newName;
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnFriendsListClear()
		{
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnBeginConnect()
		{
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnConnect()
		{
			this.typingInBox.Enabled = true;
			this.sendButton.Enabled = true;
		}


		/// <summary>
		/// Called when the connection is canceled
		/// </summary>
		public void OnConnectCanceled()
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="forced"></param>
		public void OnDisconnect(bool forced)
		{
			this.typingInBox.Enabled = false;
			this.sendButton.Enabled = false;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="opCompleteEvent"></param>
		/// <param name="tag"></param>
		public void OnInvitedToConversation(Friend friend, OperationCompleteEvent opCompleteEvent, object tag)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="status"></param>
		public void OnChangeStatus(OnlineStatus status)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"></param>
		public void OnAddFriendToList(string username)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnRemoveFriendFromList(Friend friend)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnBlockFriend(Friend friend)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnUnblockFriend(Friend friend)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		public void OnWriteDebug(string text)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="reason"></param>
		/// <param name="enableAddCheckbox"></param>
		public void OnPromptForStrangerHasAddedMe(Friend friend, string reason, bool enableAddCheckbox)
		{
		}


		#endregion


		#region Conversation listener methods


		/// <summary>
		/// 
		/// </summary>
		public void OnConversationConnected()
		{
			FlushTextQueue();
			connectionInProgress = false;
			conversationConnected = true;
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnConversationDisconnected()
		{
			conversationConnected = false;
		}


		private delegate void ShowFormHandler();


		/// <summary>
		/// 
		/// </summary>
		private void ShowForm()
		{
			this.Show();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		public void OnUserSay(string text)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="text"></param>
		public void OnFriendSay(Friend friend, string text)
		{
			if (text != null && text != string.Empty && text != "")
			{
				this.statusBar.Text = "";
				this.typingNotifyTimer.Stop();

				protocol.Invoke(new ShowFormHandler(ShowForm));
				this.WriteToConversationBox(friend.DisplayName, text, this.friendSpeakColour);

				// only flash or play sound if this is not the active form
				if (WinForms.Form.ActiveForm != this)
				{
					switch (GlobalSettings.Instance().FriendMessageEvent)
					{
						default:
						case FriendMessageEvent.DoNothing:
							break;

						case FriendMessageEvent.FlashWindow:
						{
							NativeMethods.FlashWindow(this,
								(uint)GlobalSettings.Instance().NumTimesToFlashOnMessageReceived,
								FlashWindowFlags.FlashTray);
						}
							break;
					}

					if (GlobalSettings.Instance().PlaySoundOnMessageReceived)
					{
						NativeMethods.PlaySoundFromFile(GlobalSettings.Instance().SoundOnMessageReceived,
							PlaySoundFlags.NoDefault | PlaySoundFlags.Asynchronous);
					}
				}
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnFriendJoin(Friend friend)
		{
			this.WriteEventToConvoBox(friend.Username + " has joined the conversation");
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnFriendLeave(Friend friend)
		{
			// if the original friend has left, set the need reconnect flag to true.
			if (friend.Equals(this.originalFriend))
			{
				this.conversationConnected = false;
			}

			this.WriteEventToConvoBox(friend.Username + " has left the conversation");
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnTypingNotification(Friend friend)
		{
			this.statusBar.Text = friend.Username + " is typing a message";

			// start timer
			typingNotifyTimer.Stop();
			typingNotifyTimer.Start();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnTypingNotifyTimerElapse(object sender, System.Timers.ElapsedEventArgs args)
		{
			this.statusBar.Text = "";
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="filename"></param>
		public void OnFileSendInvitation(Friend friend, string filename)
		{
		}


		#endregion


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolBar_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
			switch ((string)e.Button.Tag)
			{
				case "SendFile":
				{
					WinForms.OpenFileDialog fileDialog = new WinForms.OpenFileDialog();

					fileDialog.CheckFileExists = fileDialog.CheckPathExists = true;
					fileDialog.Multiselect = false;
					fileDialog.Filter = "All files (*.*)|*.*";
					fileDialog.FilterIndex = 0;

					if (fileDialog.ShowDialog(this) == WinForms.DialogResult.OK)
					{
						IO.Stream stream = fileDialog.OpenFile();
						this.conversation.SendFile(stream, new OperationCompleteEvent());
						stream.Close();
					}
				}
					break;
			}
		}
	}
}
