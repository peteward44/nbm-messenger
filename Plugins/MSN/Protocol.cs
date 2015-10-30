using System;
using System.Collections;
using Timers = System.Timers;
using NBM.Plugin;

using System.Threading;

using Regex = System.Text.RegularExpressions;
using WinForms = System.Windows.Forms;

// 9/6/03

/// <summary>
/// Summary description for Protocol.
/// </summary>
public class Protocol : IProtocol
{
	private readonly string[] supportedProtocols =
		new string[] { "MSNP8", "CVR0" };

	private readonly string nexusUri = @"https://nexus.passport.com:443/rdr/pprdr.asp";

	internal MessageRouter notifyRouter, dispatchRouter;
	
	internal ProtocolControl control;
	private Settings settings;

	private Timers.Timer timer = new Timers.Timer(1000 * 60); // ping timer, every 1 minute
	private Timers.Timer timeoutTimer = new Timers.Timer(1000 * 60 * 2); // ping timeout timer, every 2 minutes


	public Protocol(ProtocolControl control, ProtocolSettings settings)
	{
		this.control = control;
		this.settings = (Settings)settings;

		this.timeoutTimer.AutoReset = false;
		this.timeoutTimer.Elapsed += new Timers.ElapsedEventHandler(OnTimeoutElapsed);
		this.timer.AutoReset = true;
		this.timer.Elapsed += new Timers.ElapsedEventHandler(OnTimerElapse);
	}


	public void Connect(OperationCompleteEvent opCompleteEvent)
	{
		try
		{
			Proxy.IConnection con = this.control.CreateConnection();
			con.Connect("", 0, settings.ServerHost, settings.ServerPort, Proxy.ConnectionType.Tcp);
			dispatchRouter = new MessageRouter(this, con, null, null);
			DoAuthentication(dispatchRouter, opCompleteEvent);
		}
		catch
		{
			opCompleteEvent.Execute(new ConnectionCompleteArgs(ConnectionFailedReport.ConnectionFailed));
		}
	}


	/// <summary>
	/// Implemented by plugins to cancel the current connection
	/// </summary>
	public void CancelConnection(OperationCompleteEvent opCompleteEvent)
	{
		if (this.dispatchRouter != null)
		{
			this.dispatchRouter.ExpectingDisconnection = true;
			this.dispatchRouter.Close();
		}

		if (this.notifyRouter != null)
		{
			this.notifyRouter.ExpectingDisconnection = true;
			this.notifyRouter.Close();
		}

		timer.Stop();
		timeoutTimer.Stop();

		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	public void Disconnect(OperationCompleteEvent opCompleteEvent)
	{
		if (this.notifyRouter != null)
		{
			this.notifyRouter.ExpectingDisconnection = true;
			SendNotifyMessage(Message.ConstructMessage("OUT", string.Empty, string.Empty, false));
			if (this.notifyRouter != null)
				this.notifyRouter.Close();
		}

		timer.Stop();
		timeoutTimer.Stop();

		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	private void RegisterEvents()
	{
		this.notifyRouter.AddCodeEvent("ILN", new ResponseReceivedHandler(OnUserAdd), null);
		this.notifyRouter.AddCodeEvent("RNG", new ResponseReceivedHandler(OnInvite), null);
		this.notifyRouter.AddCodeEvent("NLN", new ResponseReceivedHandler(OnUserOnline), null);
		this.notifyRouter.AddCodeEvent("FLN", new ResponseReceivedHandler(OnUserOffline), null);
		this.notifyRouter.AddCodeEvent("OUT", new ResponseReceivedHandler(OnOutReceived), null);
		this.notifyRouter.AddCodeEvent("QNG", new ResponseReceivedHandler(OnPongReceived), null);

		this.notifyRouter.AddCodeEvent("ADD", new ResponseReceivedHandler(OnReverseListAdd), null);
		this.notifyRouter.AddCodeEvent("REM", new ResponseReceivedHandler(OnReverseListRemove), null);

		#region Errors

		this.notifyRouter.AddCodeEvent("200", new ResponseReceivedHandler(OnErrorReceived), "Syntax error");
		this.notifyRouter.AddCodeEvent("201", new ResponseReceivedHandler(OnErrorReceived), "Invalid parameter");
		this.notifyRouter.AddCodeEvent("205", new ResponseReceivedHandler(OnErrorReceived), "Invalid user");
		this.notifyRouter.AddCodeEvent("206", new ResponseReceivedHandler(OnErrorReceived), "Domain name missing");
		this.notifyRouter.AddCodeEvent("207", new ResponseReceivedHandler(OnErrorReceived), "Already logged in");
		this.notifyRouter.AddCodeEvent("208", new ResponseReceivedHandler(OnErrorReceived), "Invalid username");
		this.notifyRouter.AddCodeEvent("209", new ResponseReceivedHandler(OnErrorReceived), "Invalid fusername");
		this.notifyRouter.AddCodeEvent("210", new ResponseReceivedHandler(OnErrorReceived), "User list full");
		this.notifyRouter.AddCodeEvent("215", new ResponseReceivedHandler(OnErrorReceived), "User already there");
		this.notifyRouter.AddCodeEvent("216", new ResponseReceivedHandler(OnErrorReceived), "User already on list");
		this.notifyRouter.AddCodeEvent("217", new ResponseReceivedHandler(OnErrorReceived), "User not online");
		this.notifyRouter.AddCodeEvent("218", new ResponseReceivedHandler(OnErrorReceived), "Already in mode");
		this.notifyRouter.AddCodeEvent("219", new ResponseReceivedHandler(OnErrorReceived), "User is in the opposite list");

		this.notifyRouter.AddCodeEvent("231", new ResponseReceivedHandler(OnErrorReceived), "Tried to add a contact to a group that doesn't exist");

		this.notifyRouter.AddCodeEvent("280", new ResponseReceivedHandler(OnErrorReceived), "Switchboard failed");
		this.notifyRouter.AddCodeEvent("281", new ResponseReceivedHandler(OnErrorReceived), "Transfer to switchboard failed");

		this.notifyRouter.AddCodeEvent("300", new ResponseReceivedHandler(OnErrorReceived), "Required field missing");
		this.notifyRouter.AddCodeEvent("302", new ResponseReceivedHandler(OnErrorReceived), "Not logged in");

		this.notifyRouter.AddCodeEvent("500", new ResponseReceivedHandler(OnErrorReceived), "Internal server error");
		this.notifyRouter.AddCodeEvent("501", new ResponseReceivedHandler(OnErrorReceived), "Database server error");

		this.notifyRouter.AddCodeEvent("510", new ResponseReceivedHandler(OnErrorReceived), "File operation failed");
		this.notifyRouter.AddCodeEvent("520", new ResponseReceivedHandler(OnErrorReceived), "Memory allocation failed");

		this.notifyRouter.AddCodeEvent("540", new ResponseReceivedHandler(OnErrorReceived), "Wrong CHL value sent to server");

		this.notifyRouter.AddCodeEvent("600", new ResponseReceivedHandler(OnErrorReceived), "Server is busy");
		this.notifyRouter.AddCodeEvent("601", new ResponseReceivedHandler(OnErrorReceived), "Server is unavaliable");
		this.notifyRouter.AddCodeEvent("602", new ResponseReceivedHandler(OnErrorReceived), "Peer nameserver is down");
		this.notifyRouter.AddCodeEvent("603", new ResponseReceivedHandler(OnErrorReceived), "Database connection failed");
		this.notifyRouter.AddCodeEvent("604", new ResponseReceivedHandler(OnErrorReceived), "Server is going down");

		this.notifyRouter.AddCodeEvent("707", new ResponseReceivedHandler(OnErrorReceived), "Could not create connection");
		this.notifyRouter.AddCodeEvent("710", new ResponseReceivedHandler(OnErrorReceived), "CVR parameters either unknown or not allowed");
		this.notifyRouter.AddCodeEvent("711", new ResponseReceivedHandler(OnErrorReceived), "Write is blocking");
		this.notifyRouter.AddCodeEvent("712", new ResponseReceivedHandler(OnErrorReceived), "Session is overloaded");
		this.notifyRouter.AddCodeEvent("713", new ResponseReceivedHandler(OnErrorReceived), "Too many active users");
		this.notifyRouter.AddCodeEvent("714", new ResponseReceivedHandler(OnErrorReceived), "Too many sessions");
		this.notifyRouter.AddCodeEvent("715", new ResponseReceivedHandler(OnErrorReceived), "Not expected");
		this.notifyRouter.AddCodeEvent("717", new ResponseReceivedHandler(OnErrorReceived), "Bad friend file");

		this.notifyRouter.AddCodeEvent("911", new ResponseReceivedHandler(OnErrorReceived), "Authentication failed");
		this.notifyRouter.AddCodeEvent("913", new ResponseReceivedHandler(OnErrorReceived), "Not allowed when offline");

		this.notifyRouter.AddCodeEvent("920", new ResponseReceivedHandler(OnErrorReceived), "Not accepting new users");
		this.notifyRouter.AddCodeEvent("924", new ResponseReceivedHandler(OnErrorReceived), "Passport account not yet verified");

		#endregion
	}


	#region Send notify message methods


	private void SendNotifyMessage(Message message)
	{
		SendNotifyMessage(message, null, null);
	}


	private void SendNotifyMessage(Message message, bool addLineEnding)
	{
		SendNotifyMessage(message, null, null, true);
	}


	private void SendNotifyMessage(Message message, ResponseReceivedHandler response, object tag)
	{
		SendNotifyMessage(message, response, tag, true);
	}


	private void SendNotifyMessage(Message message, ResponseReceivedHandler response, object tag, bool addLineEnding)
	{
		this.notifyRouter.SendMessage(message, response, tag);

		// restart timer
		timer.Stop();
		timer.Start();
	}


	#endregion


	private void OnOutReceived(MessageRouter router, Message message, object tag)
	{
		// server is about to disconnect us
		this.notifyRouter.Close();
		this.notifyRouter = null;
		control.ForcedDisconnect();
		timer.Stop();
		timeoutTimer.Stop();
	}


	private void OnErrorReceived(MessageRouter router, Message message, object tag)
	{
		string errorMessage = (string)tag;
//		control.DisplayError(errorMessage, false);
	}


	#region Authentication methods


	private void DoAuthentication(MessageRouter router, OperationCompleteEvent opCompleteEvent)
	{
		string versionString = string.Join(" ", supportedProtocols);

		// send supported versions
		router.SendMessage(Message.ConstructMessage("VER", versionString), new ResponseReceivedHandler(OnVersionResponse), opCompleteEvent);
	}


	private void OnVersionResponse(MessageRouter router, Message message, object tag)
	{
		router.RemoveMessageEvent(message);

		// send CVR command - describes our local machine and client version
		router.SendMessage(
			Message.ConstructMessage("CVR", "0x0409 winnt 5.1 i386 MSMSGS 4.7.2009 WindowsMessenger " + settings.Username),
			new ResponseReceivedHandler(OnCvrResponse),
			tag);
	}


	private void OnCvrResponse(MessageRouter router, Message message, object tag)
	{
		router.RemoveMessageEvent(message);
		router.SendMessage(Message.ConstructMessage("USR", "TWN I " + settings.Username), new ResponseReceivedHandler(OnUsrResponse), tag);
	}


	private void OnUsrResponse(MessageRouter router, Message message, object tag)
	{
		router.RemoveMessageEvent(message);
		OperationCompleteEvent op = (OperationCompleteEvent)tag;

		// see if its an XFR or a USR code.
		switch (message.Code)
		{
			case "XFR": // we need to connect to a notification server
			{
				// get ip and port of notification server from response string
				Regex.Regex regex = new Regex.Regex(
					@"NS (?<notifyServerIP>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}):(?<notifyServerPort>\d{1,5})" + 
					@"\s+\d+\s+" + 
					@"(?<currentNotifyIP>\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}):(?<currentNotifyPort>\d{1,5})");
				Regex.Match match = regex.Match(message.Arguments);

				if (!match.Success)
					op.Execute(new ConnectionCompleteArgs(ConnectionFailedReport.InvalidProtocol));
				else
				{
					string notifyIP = match.Groups["notifyServerIP"].Value;
					int notifyPort = int.Parse(match.Groups["notifyServerPort"].Value);

					// connect to notification server
					if (this.notifyRouter != null)
						this.notifyRouter.Close();

					try
					{
						Proxy.IConnection con = control.CreateConnection();
						con.Connect("", 0, notifyIP, notifyPort, Proxy.ConnectionType.Tcp);
						this.notifyRouter = new MessageRouter(this, con,
							new ResponseReceivedHandler(OnDefaultNotifyMessage),
							new ResponseReceivedHandler(OnForcedDisconnect));
					}
					catch
					{
						op.Execute(new ConnectionCompleteArgs(ConnectionFailedReport.ConnectionFailed));
						return;
					}

					RegisterEvents();

					// go through authentication again, this time with the new notification server
					DoAuthentication(this.notifyRouter, (OperationCompleteEvent)tag);
				}
			}
				break;

			case "USR": // we need to do encrypt and send password
			{
				string challenge = message.Arguments.Substring(6);

				// we need to authenticate with passport server
				string url = this.settings.PassportURL;
				string ticket;

				if (url != string.Empty || url != null || url != "")
				{
					try
					{
						ticket = Passport.Login(url, challenge, settings.Username, settings.Password);
					}
					catch
					{
						// passport url is out of date
						url = Passport.GetLoginAddress(this.nexusUri);
						ticket = Passport.Login(url, challenge, settings.Username, settings.Password);
					}
				}
				else
				{
					url = Passport.GetLoginAddress(this.nexusUri);
					ticket = Passport.Login(url, challenge, settings.Username, settings.Password);
				}

				if (ticket == null || ticket == string.Empty || ticket == "")
				{
					// wrong username / password
					op.Execute(new ConnectionCompleteArgs(ConnectionFailedReport.AuthFailed));
					return;
				}

				// send ticket command
				router.SendMessage(Message.ConstructMessage("USR", "TWN S " + ticket), new ResponseReceivedHandler(OnPasswordSentResponse), tag);
			}
				break;
		}
	}


	private void OnPasswordSentResponse(MessageRouter router, Message message, object tag)
	{
		router.RemoveMessageEvent(message);

		OperationCompleteEvent op = (OperationCompleteEvent)tag;

		switch (message.Code)
		{
			case "USR":
			{
				// find our screen name
				Regex.Regex regex = new Regex.Regex(@"OK (?<email>\S+)\s+(?<screenName>\S+)");
				Regex.Match match = regex.Match(message.Header);
		
				if (!match.Success)
					op.Execute(new ConnectionCompleteArgs(ConnectionFailedReport.InvalidProtocol));
				else
				{
					settings.DisplayName = this.UriUnquote(match.Groups["screenName"].Value);

					// now we have to set our initial status.
					this.ChangeStatus(this.settings.Status, new ResponseReceivedHandler(OnInitialStatusSet), tag);
				}
			}
				break;
			case "911":
				// authentication failed
				op.Execute(new ConnectionCompleteArgs(ConnectionFailedReport.AuthFailed));
				break;
		}
	}


	private void OnInitialStatusSet(MessageRouter router, Message message, object tag)
	{
		router.RemoveMessageEvent(message);

		if (message.Code == "CHG")
		{
			SyncContactList((OperationCompleteEvent)tag);

			// start ping timer
			timer.Start();
		}
		else
		{
			OnUserAdd(router, message, tag);
		}
	}


	#endregion


	#region Contact list synchronisation


	private void SyncContactList(OperationCompleteEvent opCompleteEvent)
	{
		// send sync command
		notifyRouter.SendMessage(
			Message.ConstructMessage("SYN", this.settings.ContactListVersion.ToString()),
			new ResponseReceivedHandler(OnSynResponse), opCompleteEvent);
	}


	private void OnSynResponse(MessageRouter router, Message message, object tag)
	{
		router.RemoveMessageEvent(message);

		OperationCompleteEvent opCompleteEvent = (OperationCompleteEvent)tag;

		switch (message.Code)
		{
			case "SYN":
			{
				// check if our cached list is up-to-date
				Regex.Regex regex = new Regex.Regex(@"(?<serverVersion>\d+)\s*(?<numLST>\d*)\s*(?<numLSG>\d*)", Regex.RegexOptions.Compiled);
				Regex.Match match = regex.Match(message.Arguments);

				if (!match.Success)
				{
					opCompleteEvent.Execute(new ConnectionCompleteArgs(ConnectionFailedReport.InvalidProtocol));
					router.RemoveMessageEvent(message);
					return;
				}

				int serverVersion = int.Parse(match.Groups["serverVersion"].Value);

				if (serverVersion != this.settings.ContactListVersion)
				{
					int numLST = int.Parse(match.Groups["numLST"].Value);

					// it's different, so clear out the old friends list and await the new
					this.settings.ContactListVersion = serverVersion;
					this.settings.AllowList.Clear();
					this.settings.BlockList.Clear();
					this.settings.ForwardList.Clear();
					this.settings.ReverseList.Clear();

					// contact list sync commands
					this.notifyRouter.AddCodeEvent("GLP", new ResponseReceivedHandler(OnGLP));
					this.notifyRouter.AddCodeEvent("BLP", new ResponseReceivedHandler(OnBLP));

					this.notifyRouter.AddCodeEvent("LSG", new ResponseReceivedHandler(OnLSG));
					this.notifyRouter.AddCodeEvent("LST", new ResponseReceivedHandler(OnLST), new Pair(tag, numLST));
				}
				else
				{
					// otherwise we are up-to-date so connection is completed.

					// check if any of the friends in the reverse list are not in any of the others
					foreach (Friend friend in this.settings.ReverseList)
					{
						if (!this.settings.AllowList.Contains(friend)
							&& !this.settings.BlockList.Contains(friend))
						{
							this.HandleNewFriend(friend);
						}
					}

					// then add each friend in the forward list to the control
					foreach (Friend friend in this.settings.ForwardList)
					{
						if (!this.control.ContainsFriend(friend.Username))
							this.control.AddFriend(friend);
					}

					opCompleteEvent.Execute(new OperationCompleteArgs());
				}
			}
				break;
		}
	}


	private void OnGLP(MessageRouter router, Message message, object tag)
	{
		/*
					The client is expected to use this value as how to behave
					when a new user is added the the RL. If the value is set to A,
					the client should prompt the user about whether to add this
					user to the AL or the BL. If the value is set to N, the client
					should automatically add the user to the AL.
					The default setting is A. Note that it is still
					the client's job to add the user to one of the lists,
					and the server will not do this automatically.
				*/
		this.settings.AutoAddFriendsToAllowList = (message.Arguments == "N");
	}


	private void OnBLP(MessageRouter router, Message message, object tag)
	{
		// i dunno
	}


	private void OnLSG(MessageRouter router, Message message, object tag)
	{
	}



	private void OnLST(MessageRouter router, Message message, object tag)
	{
		Pair pair = (Pair)tag;
		OperationCompleteEvent op = (OperationCompleteEvent)pair.First;
		int numUsersLeft = (int)pair.Second;

		Regex.Regex regex = new Regex.Regex(
			@"(?<username>\S+)\s+(?<screenName>\S+)\s+(?<listID>\d+)\s*(?<groupList>.*)");

		Regex.Match match = regex.Match(message.Arguments);

		if (match.Success)
		{
			string username = match.Groups["username"].Value;
			string screenName = this.UriUnquote(match.Groups["screenName"].Value);
			int listID = int.Parse(match.Groups["listID"].Value);

			Friend friend = new Friend(username, OnlineStatus.Offline, 0);

			friend.DisplayName = screenName;
			friend.EmailAddress = username;

			if ((listID & 1) == 1) // forward list
			{
				if (!this.settings.ForwardList.Contains(friend))
					this.settings.ForwardList.Add(friend);
			}

			if ((listID & 2) == 2) // allow list
			{
				if (!this.settings.AllowList.Contains(friend))
					this.settings.AllowList.Add(friend);
			}

			if ((listID & 4) == 4) // block list
			{
				if (!this.settings.BlockList.Contains(friend))
					this.settings.BlockList.Add(friend);
			}

			if ((listID & 8) == 8) // reverse list
			{
				if (!this.settings.ReverseList.Contains(friend))
					this.settings.ReverseList.Add(friend);

				// if this person isn't on the allow or blocking list, then they have added
				// me to their forward list. Inform the user
				if (!this.settings.AllowList.Contains(friend)
					&& !this.settings.BlockList.Contains(friend))
				{
					this.HandleNewFriend(friend);
				}
			}
		}

		// check if we have finished yet
		if (--numUsersLeft == 0)
		{
			foreach (Friend friend2 in this.settings.ForwardList)
			{
				friend2.Blocked = this.settings.BlockList.Contains(friend2);

				if (!this.control.ContainsFriend(friend2.Username))
					this.control.AddFriend(friend2);
			}

			op.Execute(new ConnectionCompleteArgs());
		}
		else // replace number of users left
			router.AddCodeEvent("LST", new ResponseReceivedHandler(OnLST), new Pair(op, numUsersLeft));
	}


	#endregion


	private void HandleNewFriend(Friend friend)
	{
		if (!this.settings.AutoAddFriendsToAllowList)
		{
			// prompt to see if the user wants the person to see them or not
			// the PromptForStrangerHasAddedMeResponse() will
			this.control.PromptForStrangerHasAddedMe(friend, string.Empty, !this.settings.ForwardList.Contains(friend));
		}
		else
		{
			// add user to allow list without prompting
			this.AddContactToList(ListType.Allow, friend, true);
		}
	}


	private void OnForcedDisconnect(MessageRouter router, Message message, object tag)
	{
		if (this.notifyRouter != null)
			this.notifyRouter.Close();
		this.notifyRouter = null;
		control.ForcedDisconnect();
		timer.Stop();
		timeoutTimer.Stop();
	}


	private string UriUnquote(string str)
	{
		string newStr = string.Empty;

		// convert screen name from uri encoding
		int index = 0;
		while (index < str.Length)
		{
			newStr += Uri.HexUnescape(str, ref index);
		}

		return newStr;
	}


	private void OnUserAdd(MessageRouter router, Message message, object tag)
	{
		Regex.Regex regex = new Regex.Regex(@"(?<status>\w+)\s+(?<emailAddress>\S+)\s+(?<screenName>\S+)\s+(?<version>\d+)");
		Regex.Match match = regex.Match(message.Arguments);

		if (match.Success)
		{
			string status = match.Groups["status"].Value;
			string emailAddress = match.Groups["emailAddress"].Value;
			string screenName = this.UriUnquote(match.Groups["screenName"].Value);

			OnlineStatus s = this.ConvertStringToStatus(status);

			Friend friend = control.GetFriend(emailAddress);

			if (friend == null)
			{
				friend = new Friend(emailAddress, s, 0);
				friend.DisplayName = screenName;
				friend.EmailAddress = emailAddress;
				control.AddFriend(friend);
			}
			else
			{
				friend.Status = s;
				friend.DisplayName = screenName;
			}
		}
	}


	private void OnInvite(MessageRouter router, Message message, object tag)
	{
		Regex.Regex regex = new Regex.Regex(@"RNG\s+(?<sessionID>\d+)\s+(?<sbIP>\d+\.\d+\.\d+\.\d+):(?<sbPort>\d+)\s+CKI\s+(?<hash>\S+)\s+(?<emailAddress>\S+)\s+(?<screenName>[^\r$]*)");
		Regex.Match match = regex.Match(message.RawMessage);

		if (match.Success)
		{
			string sbIP = match.Groups["sbIP"].Value;
			int sbPort = int.Parse(match.Groups["sbPort"].Value);
			string sessionID = match.Groups["sessionID"].Value;
			string hash = match.Groups["hash"].Value;
			string email = match.Groups["emailAddress"].Value;
			string screenName = this.UriUnquote(match.Groups["screenName"].Value);

			Friend friend;

			if (this.control.ContainsFriend(email))
			{
				friend = control.GetFriend(email);
			}
			else
			{
				friend = new Friend(email, OnlineStatus.Online, 0);
				friend.DisplayName = this.UriUnquote(screenName);
				friend.EmailAddress = email;
			}

			// connect to switchboard
			control.StartInvitedConversation(friend, new OperationCompleteEvent(), new object[] { sbIP, sbPort, hash, sessionID });
		}
	}


	private void OnReverseListAdd(MessageRouter router, Message message, object tag)
	{
		Friend f = null;
		HandleListChangeMessage(message, f);
		HandleNewFriend(f);
	}


	private void OnReverseListRemove(MessageRouter router, Message message, object tag)
	{
		HandleListChangeMessage(message);
	}


	private void OnDefaultNotifyMessage(MessageRouter router, Message message, object tag)
	{
		//		WinForms.MessageBox.Show(message.RawMessage);
	}


	#region Status methods


	private OnlineStatus ConvertStringToStatus(string status)
	{
		switch (status)
		{
			case "NLN":
				return OnlineStatus.Online;

			case "AWY":
				return OnlineStatus.Away;

			case "BSY":
				return OnlineStatus.Busy;

			case "HDN":
				return OnlineStatus.AppearOffline;

			case "FLN":
				return OnlineStatus.Offline;

			case "IDL":
				return OnlineStatus.Idle;
			
			default:
				return OnlineStatus.Away;
		}
	}


	private string ConvertStatusToString(OnlineStatus status)
	{
		switch (status)
		{
			default:
			case OnlineStatus.Online:
				return "NLN";

			case OnlineStatus.Away:
				return "AWY";

			case OnlineStatus.Busy:
				return "BSY";

			case OnlineStatus.AppearOffline:
				return "HDN";

			case OnlineStatus.Offline:
				return "FLN";
		}
	}


	private void ChangeStatus(OnlineStatus status)
	{
		ChangeStatus(status, null, null);
	}


	public void ChangeStatus(OnlineStatus status, OperationCompleteEvent opCompleteEvent)
	{
		this.ChangeStatus(status, new ResponseReceivedHandler(OnStatusChangeComplete), opCompleteEvent);
	}


	private void ChangeStatus(OnlineStatus status, ResponseReceivedHandler responseHandler)
	{
		this.ChangeStatus(status, responseHandler, null);
	}


	private void ChangeStatus(OnlineStatus status, ResponseReceivedHandler responseHandler, object tag)
	{
		SendNotifyMessage(
			Message.ConstructMessage("CHG", this.ConvertStatusToString(status)),
			responseHandler, tag);
	}


	private void OnStatusChangeComplete(MessageRouter router, Message message, object tag)
	{
		((OperationCompleteEvent)tag).Execute(new OperationCompleteArgs());
	}


	#endregion


	#region Ping methods


	private void OnTimerElapse(object sender, Timers.ElapsedEventArgs e)
	{
		// send ping
		this.SendNotifyMessage(Message.ConstructMessage("PNG", string.Empty, string.Empty, false));

		// start timeout timer
		this.timeoutTimer.Start();
	}


	private void OnPongReceived(MessageRouter router, Message message, object tag)
	{
		// stop timeout timer
		this.timeoutTimer.Stop();
	}


	private void OnTimeoutElapsed(object sender, Timers.ElapsedEventArgs e)
	{
		System.Diagnostics.Debug.WriteLine("Timeout detected");
		// we havent received a pong in the specified time, assume disconnection
		control.ForcedDisconnect();
	}


	#endregion


	#region Friend status change handlers


	private void OnUserOnline(MessageRouter router, Message message, object tag)
	{
		Regex.Regex regex = new Regex.Regex(@"(?<status>\S+)\s+(?<email>\S+)\s+(?<screenName>\S+)");
		Regex.Match match = regex.Match(message.Arguments);

		if (match.Success)
		{
			string status = match.Groups["status"].Value;
			string username = match.Groups["email"].Value;
			string screenName = this.UriUnquote(match.Groups["screenName"].Value);

			if (control.ContainsFriend(username))
			{
				Friend f = control.GetFriend(username);

				f.DisplayName = screenName;
				f.Status = this.ConvertStringToStatus(status);
			}
			else
			{
				Friend friend = new Friend(username, this.ConvertStringToStatus(status), 0);
				friend.DisplayName = screenName;
				friend.EmailAddress = username;
				control.AddFriend(friend);
			}
		}
	}


	private void OnUserOffline(MessageRouter router, Message message, object tag)
	{
		string username = message.Arguments;

		Friend friend = control.GetFriend(username);

		if (friend != null)
		{
			friend.Status = OnlineStatus.Offline;
		}
	}


	#endregion


	#region Block methods


	public void UnblockFriend(Friend friend, OperationCompleteEvent opCompleteEvent)
	{
		this.AddContactToList(ListType.Block, friend, false);
		this.AddContactToList(ListType.Allow, friend, true);

		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	public void BlockFriend(Friend friend, OperationCompleteEvent opCompleteEvent)
	{
		this.AddContactToList(ListType.Allow, friend, false);
		this.AddContactToList(ListType.Block, friend, true);

		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	#endregion


	#region Add / Delete friends from contact list methods


	public void AddFriendToList(string username, string message,
		OperationCompleteEvent opCompleteEvent)
	{
		Friend f = new Friend(username, OnlineStatus.Offline, 0);

		this.AddContactToList(ListType.Allow, f, true);
		this.AddContactToList(ListType.Forward, f, true);

		this.control.AddFriend(f);

		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	public void RemoveFriendFromList(Friend friend, OperationCompleteEvent opCompleteEvent)
	{
		this.AddContactToList(ListType.Allow, friend, false);
		this.AddContactToList(ListType.Forward, friend, false);

		this.control.RemoveFriend(friend);

		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	#endregion


	#region Add contacts to different lists methods


	private enum ListType
	{
		Forward, Reverse, Allow, Block
	}


	private void AddContactToList(ListType listType, Friend friend, bool add)
	{
		AddContactToList(listType, friend, add, null);
	}


	private void AddContactToList(ListType listType, Friend friend, bool add, ResponseReceivedHandler handler)
	{
		AddContactToList(listType, friend, add, handler, null);
	}


	private string ConvertListTypeToString(ListType listType)
	{
		switch (listType)
		{
			case ListType.Allow:
				return "AL";
			case ListType.Block:
				return "BL";
			default:
			case ListType.Forward:
				return "FL";
			case ListType.Reverse:
				return "RL";
		}
	}


	private ListType ConvertStringToListType(string str)
	{
		switch (str)
		{
			case "AL":
				return ListType.Allow;
			case "BL":
				return ListType.Block;
			default:
			case "FL":
				return ListType.Forward;
			case "RL":
				return ListType.Reverse;
		}
	}
	

	private void AddContactToList(ListType listType, Friend friend, bool add, ResponseReceivedHandler handler, object tag)
	{
		string contactListString = this.ConvertListTypeToString(listType);
		string additional = string.Empty;

		switch (listType)
		{
			case ListType.Forward:
				additional = " 0";
				break;
		}

		this.notifyRouter.SendMessage(
			Message.ConstructMessage(add ? "ADD" : "REM",
			contactListString + " " + friend.Username + (add ? " " + friend.Username : "") + additional),
			new ResponseReceivedHandler(OnAddContactToListResponse),
			new object[] { friend, handler, tag }
			);
	}


	private bool HandleListChangeMessage(Message message)
	{
		return HandleListChangeMessage(message, null);
	}


	private bool HandleListChangeMessage(Message message, Friend friend)
	{
		Regex.Regex regex = new Regex.Regex(@"(?<listType>\w{2})\s+(?<contactListVersion>\d+)\s+(?<username>\S+)\s*(?<groupNumber>\d*)");
		Regex.Match match = regex.Match(message.Arguments);

		if (match.Success)
		{
			// get new contact list version number and update local friends list
			this.settings.ContactListVersion = int.Parse(match.Groups["contactListVersion"].Value);

			if (friend == null)
				friend = new Friend(match.Groups["username"].Value, OnlineStatus.Offline);

			ListType listType = this.ConvertStringToListType(match.Groups["listType"].Value);

			if (message.Code == "ADD")
			{
				switch (listType)
				{
					case ListType.Allow:
						this.settings.AllowList.Add(friend);
						break;
					case ListType.Block:
						this.settings.BlockList.Add(friend);
						break;
					case ListType.Forward:
						this.settings.ForwardList.Add(friend);
						break;
					case ListType.Reverse:
						this.settings.ReverseList.Add(friend);
						break;
				}
			}
			else
			{
				switch (listType)
				{
					case ListType.Allow:
						this.settings.AllowList.Remove(friend);
						break;
					case ListType.Block:
						this.settings.BlockList.Remove(friend);
						break;
					case ListType.Forward:
						this.settings.ForwardList.Remove(friend);
						break;
					case ListType.Reverse:
						this.settings.ReverseList.Remove(friend);
						break;
				}
			}

			return true;
		}
		else
			return false;
	}


	private void OnAddContactToListResponse(MessageRouter router, Message message, object tag)
	{
		object[] p = (object[])tag;
		Friend friend = (Friend)p[0];
		ResponseReceivedHandler handler = (ResponseReceivedHandler)p[1];
		object tag2 = p[2];

		if (HandleListChangeMessage(message, friend))
		{
			// call user-defined handler method
			if (handler != null)
				handler(router, message, new Pair(tag2, true));
		}
		else
		{
			if (handler != null)
				handler(router, message, new Pair(tag2, false));
		}
	}


	#endregion


	public void SelfDestruct()
	{
		this.notifyRouter.Close();
	}


	public void PromptForStrangerHasAddedMeResponse(Friend friend, string message,
		bool allowUser, bool addUser)
	{
		this.AddContactToList(allowUser ? ListType.Allow : ListType.Block, friend, true);

		if (addUser)
			this.AddContactToList(ListType.Forward, friend, true);
	}
}
