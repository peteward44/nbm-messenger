using System;
using System.Collections;
using NBM.Plugin;
using NBM.Plugin.Settings;

using WinForms = System.Windows.Forms;

using System.Threading;

using Drawing = System.Drawing;

// 1/6/03

namespace NBM
{
	/// <summary>
	/// Each plugin protocol has a Protocol object.
	/// </summary>
	public class Protocol : IProtocolListener, IDisposable
	{
		private Hashtable conversationTable = new Hashtable();
		private ProtocolSettings settings;
		private volatile bool isConnected = false;
		private volatile bool isConnecting = false;
		private IProtocol protocol = null;
		private ClassFactory classFactory;
		private IStorage localStorage;
		private ProtocolServer protocolServer;
		private ProtocolControl protocolControl;
		private TreeViewEx treeView;
		private TaskbarNotifier taskbarNotify;

		private ArgThread connectionThread = null;
		private const int connectionTimeout = 10 * 1000; // 10 second timeout

		private ArrayList optionsList = new ArrayList();

		private NBM.Diagnostics.ProtocolReporter reporter;


		#region Properties


		/// <summary>
		/// An arraylist of IOptions defined by the plugin.
		/// </summary>
		public ArrayList OptionsNodes
		{
			get { return optionsList; }
		}


		/// <summary>
		/// Gets a value determining whether the current protocol is connected or not.
		/// </summary>
		public bool Connected
		{
			get { return isConnected; }
		}


		/// <summary>
		/// ClassFactory object associated with the protocol.
		/// </summary>
		public ClassFactory ClassFactory
		{
			get { return this.classFactory; }
		}


		/// <summary>
		/// ProtocolServer object associated with the protocol.
		/// </summary>
		public ProtocolServer Server
		{
			get { return protocolServer; }
		}


		/// <summary>
		/// IProtocol object associated with the protocol.
		/// </summary>
		public IProtocol IProtocol
		{
			get { return protocol; }
		}


		/// <summary>
		/// IProtocolSettings object associated with the protocol.
		/// </summary>
		public ProtocolSettings Settings
		{
			get { return settings; }
		}


		/// <summary>
		/// Name of the protocol.
		/// </summary>
		public string Name
		{
			get { return this.settings.Constants.Name; }
		}


		/// <summary>
		/// True if the user has enabled the protocol, false otherwise.
		/// </summary>
		public bool Enabled
		{
			get { return this.settings.Enabled; }
		}


		/// <summary>
		/// ProtocolReporter object associated with the protocol, the reporter is used for
		/// debugging purposes
		/// </summary>
		public NBM.Diagnostics.ProtocolReporter Reporter
		{
			get { return this.reporter; }
		}


		#endregion


		/// <summary>
		/// Constructs a Protocol object.
		/// </summary>
		/// <param name="treeView">TreeViewEx object which holds the contact list for the application</param>
		/// <param name="taskbarNotify">TaskbarNotify object which pops up on certain events</param>
		/// <param name="fileName">Filename of the plugin assembly</param>
		/// <param name="mainStorage">IStorage instance where the application's settings are stored</param>
		public Protocol(TreeViewEx treeView, TaskbarNotifier taskbarNotify, string fileName,
			IStorage mainStorage)
		{
			this.treeView = treeView;
			this.taskbarNotify = taskbarNotify;

			this.classFactory = new ClassFactory(fileName);

			// create plugin constants class
			IConstants constants = classFactory.CreateConstants();

			this.localStorage = mainStorage.CreateSubSection(constants.Name);

			// create registry key for this protocol to store the settings
			this.settings = classFactory.CreateSettings(constants, this.localStorage, this.optionsList);
			this.settings.Load();

			// create protocol control and register this class to be an event listener
			this.protocolServer = new ProtocolServer();
			this.protocolServer.AddListener(this);

			this.protocolControl = new ProtocolControl(this.protocolServer);

			this.reporter = new NBM.Diagnostics.ProtocolReporter(this);
		}


		/// <summary>
		/// Invoke a delegate on the UI thread.
		/// </summary>
		/// <param name="dele"></param>
		/// <param name="args"></param>
		public void Invoke(Delegate dele, object[] args)
		{
			this.treeView.Invoke(dele, args);
		}


		/// <summary>
		/// Invoke a delegate on the UI thread.
		/// </summary>
		/// <param name="dele"></param>
		public void Invoke(Delegate dele)
		{
			this.treeView.Invoke(dele);
		}


		/// <summary>
		/// Disposes the object.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}


		/// <summary>
		/// Disposes the object.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.reporter.Dispose();
				this.settings.Save();
				this.localStorage.Close();
			}
		}


		/// <summary>
		/// 
		/// </summary>
		~Protocol()
		{
			this.Dispose(false);
		}


		#region Protocol control event registration methods


		/// <summary>
		/// Add a listener to the protocol. The listeners receive events triggered by the plugin.
		/// </summary>
		/// <param name="listener"></param>
		public void AddListener(IProtocolListener listener)
		{
			this.protocolServer.AddListener(listener);
		}


		/// <summary>
		/// Remove a listener from the protocol.
		/// </summary>
		/// <param name="listener"></param>
		public void RemoveListener(IProtocolListener listener)
		{
			this.protocolServer.RemoveListener(listener);
		}


		#endregion


		#region Connection / Disconnection methods


		private delegate void ConnectHandler(OperationCompleteEvent opCompleteEvent);


		/// <summary>
		/// Connect the protocol.
		/// </summary>
		/// <param name="opCompleteEvent"></param>
		public void Connect(OperationCompleteEvent opCompleteEvent)
		{
			if (!this.isConnected && this.settings.Enabled)
			{
				this.isConnecting = true;

				this.protocolServer.OnBeginConnect();

				opCompleteEvent.RegisterEvent(new OperationCompleteHandler(OnConnectionComplete));

				// abort the connection thread if exists
				if (this.connectionThread != null)
				{
					try
					{
						if (this.connectionThread.IsAlive)
							this.connectionThread.Abort();
					}
					catch
					{}

					this.connectionThread = null;
				}

				// start the connection on a new thread.
				this.connectionThread = new ArgThread(new ConnectHandler(pConnect));
				this.connectionThread.Start(new object[] { opCompleteEvent });
			}
		}


		/// <summary>
		/// Handler for the thread created by Connect().
		/// </summary>
		/// <param name="opCompleteEvent"></param>
		private void pConnect(OperationCompleteEvent opCompleteEvent)
		{
			// create IProtocol derived class
			this.protocol = this.classFactory.CreateProtocol(this.protocolControl, this.settings);
			this.protocol.Connect(opCompleteEvent);
			Thread.Sleep(connectionTimeout);
		}


		/// <summary>
		/// Occurs when the connection is complete.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="tag"></param>
		private void OnConnectionComplete(OperationCompleteArgs args, object tag)
		{
			this.treeView.Invalidate();
			this.treeView.Update();

			this.isConnecting = false;

			if (args is ConnectionCompleteArgs)
			{
				ConnectionCompleteArgs a = (ConnectionCompleteArgs)args;

				if (a.Success)
				{
					this.isConnected = true;
					this.protocolServer.OnConnect();
				}
				else
				{
					this.protocolServer.OnConnectCanceled();

					switch (a.Report)
					{
						case ConnectionFailedReport.AuthFailed:
						{
							// could not connect - pop up auth failed window
							AuthFailedForm form = new AuthFailedForm(this, args.ErrorMessage);
							if (form.ShowDialog() == WinForms.DialogResult.OK)
							{
								// attempt reconnect
								Connect(new OperationCompleteEvent());
							}
						}
							break;

						default:
						case ConnectionFailedReport.InvalidProtocol:
						case ConnectionFailedReport.ConnectionFailed:

							if (GlobalSettings.Instance().ReconnectOnDisconnection)
							{
								Connect(new OperationCompleteEvent());
							}
							break;
					}
				}
			}
			else
			{
				if (args.Success)
				{
					this.isConnected = true;
					this.protocolServer.OnConnect();
				}
				else
				{
					this.protocolServer.OnConnectCanceled();

					if (GlobalSettings.Instance().ReconnectOnDisconnection)
					{
						Connect(new OperationCompleteEvent());
					}
				}
			}
		}


		/// <summary>
		/// Disconnects the protocol.
		/// </summary>
		/// <param name="opCompleteEvent"></param>
		public void Disconnect(OperationCompleteEvent opCompleteEvent)
		{
			// if disconnect is called whilst we are connecting, abort the connection thread.
			if (this.isConnecting = true || this.connectionThread != null)
			{
				opCompleteEvent.RegisterEvent(new OperationCompleteHandler(OnCancelationComplete));
				this.protocol.CancelConnection(opCompleteEvent);
			}

			if (this.isConnected)
			{
				opCompleteEvent.RegisterEvent(new OperationCompleteHandler(OnDisconnectionComplete));
				this.protocol.Disconnect(opCompleteEvent);
			}
		}


		/// <summary>
		/// Occurs when the protocol has completed canceling the connection
		/// </summary>
		/// <param name="args"></param>
		/// <param name="tag"></param>
		private void OnCancelationComplete(OperationCompleteArgs args, object tag)
		{
			this.protocolServer.OnConnectCanceled();

			try
			{
				if (this.connectionThread.IsAlive)
					this.connectionThread.Abort();
			}
			catch
			{}

			this.connectionThread = null;
		}


		/// <summary>
		/// Occurs when disconnection is complete.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="tag"></param>
		private void OnDisconnectionComplete(OperationCompleteArgs args, object tag)
		{
			if (args.Success)
			{
				this.protocolServer.OnNormalDisconnect();
				this.isConnected = false;
			}

			this.connectionThread = null;
		}
		

		#endregion


		#region Friend manipulation - Adding, removing and blocking


		/// <summary>
		/// Add a friend to the contact list.
		/// </summary>
		/// <param name="username">Username of friend to add</param>
		/// <param name="message">[optional] Friend message</param>
		/// <param name="op"></param>
		public void AddFriendToList(string username, string message, OperationCompleteEvent op)
		{
			this.protocol.AddFriendToList(username, message, op);
			this.protocolServer.AddFriendToList(username);
		}


		/// <summary>
		/// Remove a friend from the contact list.
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="op"></param>
		public void RemoveFriendFromList(Friend friend, OperationCompleteEvent op)
		{
			this.protocol.RemoveFriendFromList(friend, op);
			this.protocolServer.RemoveFriendFromList(friend);
		}


		/// <summary>
		/// Block a friend
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="op"></param>
		public void BlockFriend(Friend friend, OperationCompleteEvent op)
		{
			this.protocol.BlockFriend(friend, op);
			this.protocolServer.BlockFriend(friend);

			op.RegisterEvent(new OperationCompleteHandler(OnBlockFriendCompleted), friend);
		}


		/// <summary>
		/// When the block friend operation is completed
		/// </summary>
		/// <param name="args"></param>
		/// <param name="tag"></param>
		private void OnBlockFriendCompleted(OperationCompleteArgs args, object tag)
		{
			if (args.Success)
				((Friend)tag).Blocked = true;
		}


		/// <summary>
		/// Unblock a friend.
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="op"></param>
		public void UnblockFriend(Friend friend, OperationCompleteEvent op)
		{
			this.protocol.UnblockFriend(friend, op);
			this.protocolServer.UnblockFriend(friend);

			op.RegisterEvent(new OperationCompleteHandler(OnUnblockFriendCompleted), friend);
		}


		/// <summary>
		/// When the unblock operation is complete.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="tag"></param>
		private void OnUnblockFriendCompleted(OperationCompleteArgs args, object tag)
		{
			if (args.Success)
				((Friend)tag).Blocked = false;
		}



		#endregion


		#region Status change methods


		/// <summary>
		/// Change the user's status
		/// </summary>
		/// <param name="status"></param>
		/// <param name="opCompleteEvent"></param>
		public void ChangeStatus(OnlineStatus status, OperationCompleteEvent opCompleteEvent)
		{
			if (this.isConnected)
			{
				// if connected, change status through protocol
				opCompleteEvent.RegisterEvent(new OperationCompleteHandler(OnStatusChangeComplete), status);
				this.protocol.ChangeStatus(status, opCompleteEvent);
			}
			else
			{
				// else change the initial status
				pChangeStatus(status);
			}
		}


		/// <summary>
		/// When the status change operation is complete.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="tag"></param>
		private void OnStatusChangeComplete(OperationCompleteArgs args, object tag)
		{
			if (args.Success)
			{
				pChangeStatus((OnlineStatus)tag);
			}
		}


		/// <summary>
		/// Change the status settings and inform the listeners
		/// </summary>
		/// <param name="status"></param>
		private void pChangeStatus(OnlineStatus status)
		{
			this.protocolServer.OnChangeStatus(status);
			this.settings.Status = status;
		}


		#endregion


		#region Conversation methods


		/// <summary>
		/// Begins a conversation with the specified friend.
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="byinvite">Whether the conversation has been initiated by the user or by the plugin</param>
		/// <param name="tag">Plugin-defined object</param>
		/// <param name="opCompleteEvent"></param>
		public void StartConversation(Friend friend, bool byinvite, object tag, OperationCompleteEvent opCompleteEvent)
		{
			if (this.conversationTable.Contains(friend))
			{
				MessageForm messageForm = (MessageForm)this.conversationTable[friend];
				if (!byinvite)
					messageForm.BringToFront();
			}
			else
			{
				MessageForm messageForm = new MessageForm(this, friend);

				messageForm.Closed += new EventHandler(OnMessageFormClosed);

				if (!byinvite)
					messageForm.Show();

				messageForm.Connect(byinvite, tag, opCompleteEvent);

				conversationTable.Add(friend, messageForm);
			}
		}


		/// <summary>
		/// When a message form is closed, remove it from the conversation table.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMessageFormClosed(object sender, EventArgs e)
		{
			this.conversationTable.Remove(((MessageForm)sender).OriginalFriend);
		}


		#endregion


		#region ProtocolListener events


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
			if (this.Connected) // ignore any status changes until we have connected
			{
				if (friend.Status == OnlineStatus.Offline)
				{
					if (GlobalSettings.Instance().PlaySoundOnFriendOnline)
					{
						NativeMethods.PlaySoundFromFile(GlobalSettings.Instance().SoundOnFriendOnline,
							PlaySoundFlags.NoDefault | PlaySoundFlags.Asynchronous);
					}

					switch (GlobalSettings.Instance().FriendOnlineEvent)
					{
						default:
						case FriendOnlineEvent.DoNothing:
							break;
					
						case FriendOnlineEvent.BalloonToolTip:
							break;

						case FriendOnlineEvent.MSNPopUp:
						{
							this.taskbarNotify.Invoke(
								new ShowTaskbarNotifierHandler(ShowTaskbarNotifier),
								new object[] { "", friend.DisplayName + " (" + friend.Username + ") is Online" });
						}
							break;
					}
				}
			}
		}


		private delegate void ShowTaskbarNotifierHandler(string title, string text);


		private void ShowTaskbarNotifier(string title, string text)
		{
			this.taskbarNotify.Show(title, text, 1000, 3000, 500);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="newName"></param>
		public void OnFriendChangeDisplayName(Friend friend, string newName)
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
			this.isConnected = false;
			this.protocol = null;

			if (forced && NBM.Plugin.Settings.GlobalSettings.Instance().ReconnectOnDisconnection)
			{
				// reconnect
				Connect(new OperationCompleteEvent());
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="opCompleteEvent"></param>
		/// <param name="tag"></param>
		public void OnInvitedToConversation(Friend friend, OperationCompleteEvent opCompleteEvent, object tag)
		{
			this.StartConversation(friend, true, tag, opCompleteEvent);
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
			this.treeView.Invoke(
				new OnPromptForStrangerHasAddedMeHandler(OnPromptForStrangerHasAddedMeFunc),
				new object[] { friend, reason, enableAddCheckbox });
		}


		private delegate void OnPromptForStrangerHasAddedMeHandler(Friend friend, string reason, bool enableAddCheckbox);


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="reason"></param>
		/// <param name="enableAddCheckbox"></param>
		private void OnPromptForStrangerHasAddedMeFunc(Friend friend, string reason, bool enableAddCheckbox)
		{
			OnFriendAddForm form = new OnFriendAddForm(this.protocol, friend, reason, enableAddCheckbox);
			form.Show();
		}


		#endregion


		/// <summary>
		/// 
		/// </summary>
		public void SelfDestruct()
		{
			this.protocol.SelfDestruct();
		}
	}
}
