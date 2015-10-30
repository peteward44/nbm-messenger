using System;
using NBM.Plugin;
using System.Text;
using System.Collections;
using System.Timers;

// 15/5/03

#region Enumerations


/* Service constants */
public enum YahooService
{ /* these are easier to see in hex */
	None = 0,
	UserOnline = 1,
	UserOffline,
	UserIsAway,
	UserIsBack,
	UserIsIdle, /* 5 (placemarker) */
	Message,
	IDActivate,
	IDDeactivate,
	EMailStatus,
	UserStatus, /* 0xa */
	NewMail,
	ChatInvite,
	Calender,
	NewPersonalMail,
	NewContact,
	AddIdent, /* 0x10 */
	AddIgnore,
	Ping,
	GotGroupName, /* < 1, 36(old), 37(new) */
	SystemMessage = 0x14,
	PassThrough2 = 0x16,
	ConferenceInvite = 0x18,
	ConferenceLogon,
	ConferenceDecline,
	ConferenceLogoff,
	ConferenceAddInvite,
	ConferenceMessage,
	ChatLogon,
	ChatLogoff,
	ChatMessage = 0x20,
	GameLogon = 0x28,
	GameLogoff,
	GameMessage = 0x2a,
	FileTransfer = 0x46,
	VoiceChat = 0x4A,
	Notify,
	Verify,
	PeerToPeerFileTransfer,
	PeerToPeer = 0x4F,        /* Checks if P2P possible */
	AuthResponse = 0x54,
	List,
	Auth = 0x57,
	AddBuddy = 0x83,
	RemoveBuddy,
	IgnoreContact,    /* > 1, 7, 13 < 1, 66, 13, 0*/
	RejectContact,
	GroupRename = 0x89, /* > 1, 65(new), 66(0), 67(old) */
	ChatOnline = 0x96, /* > 109(id), 1, 6(abcde) < 0,1*/
	ChatGoto,
	ChatJoin, /* > 1 104-room 129-1600326591 62-2 */
	ChatLeave,
	ChatExit = 0x9b,
	ChatLogout = 0xa0,
	ChatPing,
	Comment = 0xa8
}


/* Message flags */
public enum YahooStatus 
{
	Error = -1,
	Available = 0,
	BRB,
	Busy,
	NotAtHome,
	NotAtDesk,
	NotInOffice,
	OnPhone,
	OnVacation,
	OutToLunch,
	SteppedOut,
	Invisible = 12,
	Custom = 99,
	Idle = 999,
	Offline = 0x5a55aa56, /* don't ask */
	Typing = 0x16
}


#endregion


public class Protocol : IProtocol
{
	private string username;

	private Settings settings;
	internal ProtocolControl control;
	private MessageRouter messageRouter;
	private UInt32 sessionID = 0;
	internal Hashtable conversationTable = new Hashtable();
	private bool needChangeStatus = false;

	private const int challengeSeedLength = 24;
	private const int challengeResponseLength = 24;
	private const char contactSeperator = ',';
	private const char contactGroupSeperator = '\n';

	private string defaultGroup = "Friends";

	private Timer pingTimer = new Timer(60 * 1000); // ping timer, ping every minute



	public Protocol(ProtocolControl control, ProtocolSettings settings)
	{
		this.control = control;
		this.settings = (Settings)settings;

		this.pingTimer.AutoReset = true;
		this.pingTimer.Elapsed += new ElapsedEventHandler(OnPingTimerElapse);
	}


	private void OnPingTimerElapse(object sender, ElapsedEventArgs e)
	{
		// send ping
		this.messageRouter.SendMessage(new Message(this.sessionID, YahooService.Ping, YahooStatus.Available));
	}


	public void Connect(OperationCompleteEvent opCompleteEvent)
	{
		this.username = settings.Username;

		// connect to the server first
		try
		{
			Proxy.IConnection conn = this.control.CreateConnection();
			conn.Connect("", 0, settings.ServerHost, settings.ServerPort, Proxy.ConnectionType.Tcp);
			messageRouter = new MessageRouter(this, conn, new ResponseReceivedHandler(OnDefaultMessage), new ResponseReceivedHandler(OnDisconnect));
		}
		catch
		{
			opCompleteEvent.Execute(new ConnectionCompleteArgs(ConnectionFailedReport.ConnectionFailed));
			return;
		}

		// register service events
		messageRouter.AddOneTimeEvent(YahooService.Verify, new ResponseReceivedHandler(OnInitialVerify), opCompleteEvent);
		messageRouter.AddOneTimeEvent(YahooService.Auth, new ResponseReceivedHandler(OnAuthResponse), opCompleteEvent);
		messageRouter.AddOneTimeEvent(YahooService.AuthResponse, new ResponseReceivedHandler(OnAuthResponseResponse), opCompleteEvent);
		messageRouter.AddOneTimeEvent(YahooService.List, new ResponseReceivedHandler(OnList), opCompleteEvent);

		messageRouter.AddPermanentEvent(YahooService.Verify, new ResponseReceivedHandler(OnVerify));
		messageRouter.AddPermanentEvent(YahooService.NewContact, new ResponseReceivedHandler(OnNewContact));

		// status messages
		messageRouter.AddOneTimeEvent(YahooService.UserOnline, new ResponseReceivedHandler(OnInitialUserOnline), opCompleteEvent);
		messageRouter.AddPermanentEvent(YahooService.UserOnline, new ResponseReceivedHandler(OnUserOnline));
		messageRouter.AddPermanentEvent(YahooService.UserOffline, new ResponseReceivedHandler(OnUserOffline));

		messageRouter.AddPermanentEvent(YahooService.UserIsAway, new ResponseReceivedHandler(OnUserIsAway));
		messageRouter.AddPermanentEvent(YahooService.UserIsBack, new ResponseReceivedHandler(OnUserIsBack));

		// conversation messages
		messageRouter.AddPermanentEvent(YahooService.Message, new ResponseReceivedHandler(OnMessage));
		messageRouter.AddPermanentEvent(YahooService.Notify, new ResponseReceivedHandler(OnNotifyMessage));

		// send verify message
		Message verifyMessage = new Message(0, YahooService.Verify);
		messageRouter.SendMessage(verifyMessage);
	}


	private void OnInitialVerify(MessageRouter router, Message message, object tag)
	{
		// construct login message
		Message loginMessage = new Message(0, YahooService.Auth);
		loginMessage.AddArgument(1, this.username);

		// send it
		messageRouter.SendMessage(loginMessage);
	}


	private void OnVerify(MessageRouter router, Message message, object tag)
	{
		// do nothing
	}


	private void OnMessage(MessageRouter router, Message message, object tag)
	{
		// just received a message, find the username and route it to the appropriate Conversation object
		string username = message.GetArgumentString(4);
		if (username == null)
			username = message.GetArgumentString(5);

		if (this.conversationTable.Contains(username))
		{
			((Conversation)this.conversationTable[username]).RouteMessage(message);
		}
		else
		{
			// start the conversation ourselves by invite
			this.control.StartInvitedConversation(
				new Friend(username.ToLower(), OnlineStatus.Online, string.Empty),
				new OperationCompleteEvent(), message
				);
		}
	}


	internal void Say(string friendname, string text, OperationCompleteEvent opCompleteEvent)
	{
		Message message = new Message(this.sessionID, YahooService.Message);

		message.AddArgument(1, this.username);
		message.AddArgument(5, friendname);
		message.AddArgument(14, text);
		message.AddArgument(97, "1");
		message.AddArgument(63, ";0");
		message.AddArgument(64, "0");

		if (messageRouter != null)
			messageRouter.SendMessage(message);
		else
			opCompleteEvent.Execute(new OperationCompleteArgs("Could not deliver message", false));
	}


	internal void AddConversation(string username, Conversation conversation)
	{
		this.conversationTable.Add(username, conversation);
	}


	private void OnInitialUserOnline(MessageRouter router, Message message, object tag)
	{
		OnUserOnline(router, message, tag);

		// start ping timer
		pingTimer.Start();

		// finally, if the initial status needs to be set, set it.
		if (needChangeStatus)
		{
			this.ChangeStatus(settings.Status, (OperationCompleteEvent)tag);
			needChangeStatus = false;
		}
		else
			((OperationCompleteEvent)tag).Execute(new OperationCompleteArgs());
	}


	private void OnUserOnline(MessageRouter router, Message message, object tag)
	{
		string lastUsername = string.Empty;

		foreach (Pair pair in message.Arguments)
		{
			int id = (int)pair.First;
			byte[] content = (byte[])pair.Second;

			string strContent = ASCIIEncoding.ASCII.GetString(content);

			switch (id)
			{
				case 7:
					lastUsername = strContent.ToLower();
					break;

				case 10:
					try
					{
						Friend f = this.control.GetFriend(lastUsername);
						if (f != null)
							f.Status = this.ConvertYahooToStatus((YahooStatus)int.Parse(strContent));

						lastUsername = string.Empty;
					}
					catch
					{}
					break;
			}
		}
	}


	private void OnUserOffline(MessageRouter router, Message message, object tag)
	{
		foreach (Pair pair in message.Arguments)
		{
			int id = (int)pair.First;
			byte[] content = (byte[])pair.Second;

			string strContent = ASCIIEncoding.ASCII.GetString(content);

			switch (id)
			{
				case 7:
				{
					Friend f = this.control.GetFriend(strContent.ToLower());
					if (f != null)
						f.Status = OnlineStatus.Offline;
				}
					break;
			}
		}
	}


	private void OnUserIsAway(MessageRouter router, Message message, object tag)
	{
		string lastUsername = string.Empty;

		foreach (Pair pair in message.Arguments)
		{
			int id = (int)pair.First;
			byte[] content = (byte[])pair.Second;

			string strContent = ASCIIEncoding.ASCII.GetString(content);

			switch (id)
			{
				case 7:
					lastUsername = strContent.ToLower();
					break;

				case 10:
					try
					{
						Friend f = this.control.GetFriend(lastUsername);
						if (f != null)
							f.Status = this.ConvertYahooToStatus((YahooStatus)int.Parse(strContent));
						lastUsername = string.Empty;
					}
					catch
					{}
					break;
			}
		}
	}


	private void OnUserIsBack(MessageRouter router, Message message, object tag)
	{
		foreach (Pair pair in message.Arguments)
		{
			int id = (int)pair.First;
			byte[] content = (byte[])pair.Second;

			string strContent = ASCIIEncoding.ASCII.GetString(content);

			switch (id)
			{
				case 7:
				{
					Friend f = this.control.GetFriend(strContent.ToLower());
					if (f != null)
						f.Status = OnlineStatus.Online;
				}
					break;
			}
		}
	}


	private void OnAuthResponse(MessageRouter router, Message message, object tag)
	{
		this.sessionID = message.Header.SessionID;

		if (message.Header.Status == YahooStatus.Error)
		{
			// authentication failed
			((OperationCompleteEvent)tag).Execute(new ConnectionCompleteArgs(ConnectionFailedReport.AuthFailed));
		}
		else
		{
			// extract challenge seed from arguments
			byte[] challengeSeed = message.GetArgument(94);

			Message authResponseMessage = new Message(this.sessionID, YahooService.AuthResponse, ConvertStatusToYahoo(settings.Status));

			// Encrypt the username & password to get the two strings we need to send
			byte[] result1, result2;
			Encryption.Encrypt(this.username, settings.Password, challengeSeed, out result1, out result2);

			authResponseMessage.AddArgument(6, result1);
			authResponseMessage.AddArgument(96, result2);
			authResponseMessage.AddArgument(0, this.username);
			authResponseMessage.AddArgument(2, string.Format("{0}", 1));
			authResponseMessage.AddArgument(1, this.username);
			authResponseMessage.AddArgument(135, "5, 5, 0, 1250");

			if (settings.Status != OnlineStatus.AppearOffline)
			{
				this.needChangeStatus = true;
				authResponseMessage.Header.Status = (YahooStatus)1515563605;
			}
			else
				authResponseMessage.Header.Status = YahooStatus.Invisible;

			messageRouter.SendMessage(authResponseMessage);
		}
	}


	private void OnAuthResponseResponse(MessageRouter router, Message message, object tag)
	{
		if (message.Header.Status == YahooStatus.Error)
		{
			// authentication failed
			((OperationCompleteEvent)tag).Execute(
				new ConnectionCompleteArgs(ConnectionFailedReport.AuthFailed));
		}
	}


	private void OnList(MessageRouter router, Message message, object tag)
	{
		// this is a comma-seperated list of people on our contact list
		string people = message.GetArgumentString(87);

		if (people != null && people[0] != 10)
		{
			string[] peopleList = null;
			string groupName = this.defaultGroup;

			if (people.IndexOf(contactSeperator) >= 0)
			{
				peopleList = people.Split(contactSeperator);
			}
			else
			{
				peopleList = new string[] { people };
			}

			int i = peopleList[0].IndexOf(":");
			if (i >= 0)
			{
				groupName = peopleList[0].Substring(0, i);
				string username = peopleList[0].Substring(i+1);
				if (username.EndsWith("\n"))
					username = username.Substring(0, username.Length-1);
				peopleList[0] = username;
			}

			foreach (string person in peopleList)
			{
				// if the string contains the group seperator then we have just hit a group boundary
				int groupIndex = person.IndexOf(contactGroupSeperator);

				if (groupIndex >= 0)
				{
					// add name before group name
					this.control.AddFriend(
						new Friend(person.Substring(0, groupIndex).ToLower(), OnlineStatus.Offline, groupName)
						);

					// add name after new group name and get the group name
					i = person.IndexOf(":");
					if (i >= 0)
					{
						int j = person.IndexOf("\n");
						if (j >= 0)
							groupName = person.Substring(j+1, i-j-1);

						string username = person.Substring(i+1);
						if (username.EndsWith("\n"))
							username = username.Substring(0, username.Length-1);
						this.control.AddFriend(new Friend(username.ToLower(), OnlineStatus.Offline, groupName));
					}
				}
				else
				{
					this.control.AddFriend(new Friend(person.ToLower(), OnlineStatus.Offline, groupName));
				}
			}

			this.defaultGroup = groupName;
		}
	}


	private void OnDefaultMessage(MessageRouter router, Message message, object tag)
	{
	}


	private void OnDisconnect(MessageRouter router, Message message, object tag)
	{
		if (this.messageRouter != null)
			this.messageRouter.Close();
		this.messageRouter = null;
		conversationTable.Clear();
		this.control.ForcedDisconnect();
		pingTimer.Stop();
	}


	/// <summary>
	/// Implemented by plugins to cancel the current connection
	/// </summary>
	public void CancelConnection(OperationCompleteEvent opCompleteEvent)
	{
		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	public void Disconnect(OperationCompleteEvent opCompleteEvent)
	{
		this.messageRouter.ExpectingDisconnection = true;
		this.messageRouter.Close();
		this.messageRouter = null;
		conversationTable.Clear();

		opCompleteEvent.Execute(new OperationCompleteArgs());
		pingTimer.Stop();
	}


	internal void SendTypingNotification(Friend friend, OperationCompleteEvent opCompleteEvent)
	{
		Message message = new Message(this.sessionID, YahooService.Notify, YahooStatus.Typing);

		message.AddArgument(49, "TYPING");
		message.AddArgument(1, this.username);
		message.AddArgument(14, " ");
		message.AddArgument(13, "1");
		message.AddArgument(5, friend.Username);
		message.AddArgument(1002, "1");

		this.messageRouter.SendMessage(message);

		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	private void OnNotifyMessage(MessageRouter router, Message message, object tag)
	{
		string username = message.GetArgumentString(4).ToLower();
		Conversation convo = (Conversation)this.conversationTable[ username ];

		if (convo != null)
			convo.control.TypingNotification(username);
	}


	#region Status methods


	public void ChangeStatus(OnlineStatus status, OperationCompleteEvent opCompleteEvent)
	{
		if (status == OnlineStatus.Idle)
		{
			messageRouter.SendMessage(new Message(this.sessionID, YahooService.UserIsIdle));
		}
		else if (status == OnlineStatus.Online)
		{
			messageRouter.SendMessage(new Message(this.sessionID, YahooService.UserIsBack));
		}
		else
		{
			Message message = new Message(this.sessionID, YahooService.UserIsAway);

			message.AddArgument(10, ((int)ConvertStatusToYahoo(status)).ToString());
			message.AddArgument(47, "1");

			messageRouter.SendMessage(message);
		}

		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	private YahooStatus ConvertStatusToYahoo(OnlineStatus status)
	{
		switch (status)
		{
			default:
			case OnlineStatus.Online:
				return YahooStatus.Available;
			case OnlineStatus.Away:
				return YahooStatus.BRB;
			case OnlineStatus.Idle:
				return YahooStatus.Idle;
			case OnlineStatus.Busy:
				return YahooStatus.Busy;
			case OnlineStatus.AppearOffline:
				return YahooStatus.Invisible;
			case OnlineStatus.Offline:
				return YahooStatus.Offline;
		}
	}


	private OnlineStatus ConvertYahooToStatus(YahooStatus status)
	{
		switch (status)
		{
			case YahooStatus.Available:
				return OnlineStatus.Online;

			case YahooStatus.Idle:
				return OnlineStatus.Idle;

			default:
			case YahooStatus.NotAtHome:
			case YahooStatus.NotAtDesk:
			case YahooStatus.NotInOffice:
			case YahooStatus.OnPhone:
			case YahooStatus.OnVacation:
			case YahooStatus.OutToLunch:
			case YahooStatus.SteppedOut:
			case YahooStatus.Custom:
			case YahooStatus.BRB:
					return OnlineStatus.Away;

			case YahooStatus.Busy:
					return OnlineStatus.Busy;

			case YahooStatus.Invisible:
					return OnlineStatus.AppearOffline;

			case YahooStatus.Offline:
					return OnlineStatus.Offline;
		}
	}


	#endregion


	#region Blocking methods


	public void BlockFriend(Friend friend, OperationCompleteEvent opCompleteEvent)
	{
		Message message = new Message(this.sessionID, YahooService.IgnoreContact);

		message.AddArgument(1, this.settings.Username);
		message.AddArgument(7, friend.Username);
		message.AddArgument(13, "1");

		this.messageRouter.SendMessage(message);

		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	public void UnblockFriend(Friend friend, OperationCompleteEvent opCompleteEvent)
	{
		Message message = new Message(this.sessionID, YahooService.IgnoreContact);

		message.AddArgument(1, this.settings.Username);
		message.AddArgument(7, friend.Username);
		message.AddArgument(13, "2");

		this.messageRouter.SendMessage(message);

		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	#endregion


	#region Add / Remove friend from list methods


	public void AddFriendToList(string username, string friendMessage,
		OperationCompleteEvent opCompleteEvent)
	{
		Message message = new Message(this.sessionID, YahooService.AddBuddy);

		message.AddArgument(1, this.settings.Username);
		message.AddArgument(7, username);
		message.AddArgument(14, friendMessage);
		message.AddArgument(65, this.defaultGroup);

		this.messageRouter.SendMessage(message);

		this.control.AddFriend(new Friend(username, OnlineStatus.Offline, this.defaultGroup));
		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	public void RemoveFriendFromList(Friend friend, OperationCompleteEvent opCompleteEvent)
	{
		Message message = new Message(this.sessionID, YahooService.RemoveBuddy);

		message.AddArgument(1, this.settings.Username);
		message.AddArgument(7, friend.Username);
		message.AddArgument(65, (string)friend.GroupIdentifier);

		this.messageRouter.SendMessage(message);

		this.control.RemoveFriend(friend);
		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	#endregion


	public void SelfDestruct()
	{
		this.messageRouter.Close();
	}


	private void OnNewContact(MessageRouter router, Message message, object tag)
	{
		string myName = message.GetArgumentString(1);
		string friendName = message.GetArgumentString(3);
		string friendMessage = message.GetArgumentString(14);

		if (friendName != null && friendName != string.Empty && friendName != "")
		{
			this.control.PromptForStrangerHasAddedMe(new Friend(friendName.ToLower(), OnlineStatus.Offline, ""),
				friendMessage, !this.control.ContainsFriend(friendName));
		}
	}


	public void PromptForStrangerHasAddedMeResponse(Friend friend, string friendMessage, 
		bool allowUser, bool addUser)
	{
		if (!allowUser)
		{
			Message message = new Message(this.sessionID, YahooService.RejectContact);

			message.AddArgument(1, this.username);
			message.AddArgument(7, friend.Username);
			message.AddArgument(14, ""); // rejection reason

			this.messageRouter.SendMessage(message);
		}

		if (addUser)
		{
			this.AddFriendToList(friend.Username, friendMessage, new OperationCompleteEvent());
		}
	}
}
