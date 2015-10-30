using System;
using System.Collections;
using IO = System.IO;
using NBM.Plugin;
using Regex = System.Text.RegularExpressions;

// 9/6/03

/// <summary>
/// Summary description for Conversation.
/// </summary>
public class Conversation : IConversation
{
	private Protocol protocol;
	private Settings settings;
	private ConversationControl control;
	private MessageRouter sbRouter;


	private void RegisterCodeEvents()
	{
		sbRouter.AddCodeEvent("MSG", new ResponseReceivedHandler(OnMsgReceived), null);
		sbRouter.AddCodeEvent("BYE", new ResponseReceivedHandler(OnByeReceived), null);
		sbRouter.AddCodeEvent("JOI", new ResponseReceivedHandler(OnJoinReceived), null);
	}


	private void OnSwitchBoardRequestResponse(MessageRouter router, Message message, object tag)
	{
		object[] o = (object[])tag;

		Friend friend = (Friend)o[0];
		OperationCompleteEvent op = (OperationCompleteEvent)o[1];

		Regex.Regex regex = new Regex.Regex(@"SB (?<sbServerIP>\d+\.\d+\.\d+\.\d+):(?<sbServerPort>\d+) CKI (?<hash>.*$)");
		Regex.Match match = regex.Match(message.Arguments);

		if (match.Success)
		{
			string sbIP = match.Groups["sbServerIP"].Value;
			int sbPort = int.Parse(match.Groups["sbServerPort"].Value);
			string conversationID = match.Groups["hash"].Value;

			// connect
			try
			{
				Proxy.IConnection conn = this.control.CreateConnection();
				conn.Connect("", 0, sbIP, sbPort, Proxy.ConnectionType.Tcp);
				sbRouter = new MessageRouter(this.protocol, conn, null, new ResponseReceivedHandler(OnForcedDisconnect));
			}
			catch
			{
				op.Execute(new OperationCompleteArgs("Could not connect", true));
				return;
			}

			RegisterCodeEvents();

			sbRouter.SendMessage(Message.ConstructMessage("USR", this.settings.Username + " " + conversationID),
				new ResponseReceivedHandler(OnSwitchBoardUsrResponse), tag);
		}
		else
			op.Execute(new OperationCompleteArgs("Could not connect", true));

		router.RemoveMessageEvent(message);
	}


	private void OnForcedDisconnect(MessageRouter router, Message message, object tag)
	{
//		control.ForcedDisconnection();
	}


	private void OnSwitchBoardUsrResponse(MessageRouter router, Message message, object tag)
	{
		object[] objs = (object[])tag;

		Friend friend = (Friend)objs[0];
		OperationCompleteEvent e = (OperationCompleteEvent)objs[1];

		this.InviteFriend(friend, e);
		router.RemoveMessageEvent(message);
	}


	private void OnAnswerResponse(MessageRouter router, Message message, object tag)
	{
		if (message.Arguments == "OK")
		{
			((OperationCompleteEvent)tag).Execute(new OperationCompleteArgs());
			router.RemoveMessageEvent(message);
		}
		else
		{
			if (message.Code == "IRO")
			{
				Regex.Regex regex = new Regex.Regex(
					@"(?<userNumber>\d+)\s+(?<totalUsers>\d+)\s+(?<username>\S+)\s+(?<screenName>.*$)"
					);

				Regex.Match match = regex.Match(message.Arguments);

				if (match.Success)
				{
					string username = match.Groups["username"].Value;
					control.FriendJoin(username);
				}
			}
		}
	}


	private void OnMsgReceived(MessageRouter router, Message message, object tag)
	{
		// seperate message from all the crap
		string actualMessage = message.Body.Substring( message.Body.IndexOf("\r\n\r\n") + 4 );
		actualMessage.TrimEnd('\r', '\n');

		// locate content type in mime headers
		Regex.Regex contentTypeRegex = new Regex.Regex(@"\r\nContent-Type:\s+(?<contentType>[^\;\s]+)");
		Regex.Match contentTypeMatch = contentTypeRegex.Match(message.Body);

		if (contentTypeMatch.Success)
		{
			string contentType = contentTypeMatch.Groups["contentType"].Value;

			switch (contentType)
			{
				case "text/x-msmsgscontrol": // user typing notification
				{
					// get username of typing user
					Regex.Regex notifyRegex = new Regex.Regex(@"TypingUser: (?<username>[^\r]+)");
					Regex.Match notifyMatch = notifyRegex.Match(message.Body);

					if (notifyMatch.Success)
					{
						this.control.TypingNotification(notifyMatch.Groups["username"].Value);
					}
				}
					return;

				case "text/x-msmsgsinvite": // file transfer, netmeeting invitation
					return;
			}
		}	

		// find user's name
		Regex.Regex regex = new Regex.Regex(@"(?<username>\S+)\s+(?<displayName>\S+)\s+\d+");
		Regex.Match match = regex.Match(message.Header);

		if (match.Success)
		{
			string username = match.Groups["username"].Value;
			string displayName = match.Groups["displayName"].Value;

			control.FriendSay(username, actualMessage);
		}
	}


	private void OnByeReceived(MessageRouter router, Message message, object tag)
	{
		Regex.Regex regex = new Regex.Regex(@"(?<username>\S*)\s*\d*");
		Regex.Match match = regex.Match(message.Arguments);

		if (match.Success)
		{
			string username = match.Groups["username"].Value;

			control.FriendLeave(username);
		}
	}


	private void OnJoinReceived(MessageRouter router, Message message, object tag)
	{
		Regex.Regex regex = new Regex.Regex(@"(?<username>\S+)\s+(?<screenName>\S+)");
		Regex.Match match = regex.Match(message.Arguments);

		if (match.Success)
		{
			string email = match.Groups["username"].Value;
			string screenName = match.Groups["screenName"].Value;

			control.FriendJoin(email);
		}
	}


	private void OnSayAck(MessageRouter router, Message message, object tag)
	{
		switch (message.Code)
		{
			case "ACK":
				((OperationCompleteEvent)tag).Execute(new OperationCompleteArgs());
				break;
			case "NAK":
				((OperationCompleteEvent)tag).Execute(new OperationCompleteArgs("Message could not be sent", false));
				break;
			default:
				((OperationCompleteEvent)tag).Execute(new OperationCompleteArgs("Unknown error occurred", false));
				break;
		}
	}


	private void OnInviteFriendComplete(MessageRouter router, Message message, object tag)
	{
		router.RemoveMessageEvent(message);
	}


	private void OnJoinAfterInvite(MessageRouter router, Message message, object tag)
	{
		router.AddCodeEvent("JOI", new ResponseReceivedHandler(OnJoinReceived), null);
		OnJoinReceived(router, message, null);
		((OperationCompleteEvent)tag).Execute(new OperationCompleteArgs());
	}


	public Conversation(IProtocol protocol, ConversationControl control, ProtocolSettings settings)
	{
		this.protocol = (Protocol)protocol;
		this.control = control;
		this.settings = (Settings)settings;
	}


	public void Start(Friend friend, OperationCompleteEvent opCompleteEvent)
	{
		protocol.notifyRouter.SendMessage(Message.ConstructMessage("XFR", "SB"),
			new ResponseReceivedHandler(OnSwitchBoardRequestResponse),
			new object[] { friend, opCompleteEvent });
	}


	public void StartByInvitation(Friend friend, OperationCompleteEvent opCompleteEvent, object tag)
	{
		object[] objs = (object[])tag;
		string sbIP = (string)objs[0];
		int sbPort = (int)objs[1];
		string hash = (string)objs[2];
		string sessionID = (string)objs[3];

		try
		{
			Proxy.IConnection conn = this.control.CreateConnection();
			conn.Connect("", 0, sbIP, sbPort, Proxy.ConnectionType.Tcp);
			sbRouter = new MessageRouter(this.protocol, conn, null, null);

			RegisterCodeEvents();

			sbRouter.SendMessage(Message.ConstructMessage("ANS",
				this.settings.Username + " " + hash + " " + sessionID),
				new ResponseReceivedHandler(OnAnswerResponse), opCompleteEvent);
		}
		catch
		{
			opCompleteEvent.Execute(new OperationCompleteArgs("Could not connect", true));
		}
	}


	public void End(OperationCompleteEvent opCompleteEvent)
	{
		if (sbRouter != null)
		{
			sbRouter.SendMessage(Message.ConstructMessage("OUT", string.Empty, string.Empty, false));
			sbRouter.Close();
		}
		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	public void Say(string text, OperationCompleteEvent opCompleteEvent)
	{
		string mimeHeader = "MIME-Version: 1.0\r\nContent-Type: text/plain; charset=UTF-8\r\nX-MMS-IM-Format: FN=Microsoft%20Sans%20Serif; CO=000000; CS=0; PF=22\r\n\r\n";

		int length = mimeHeader.Length + text.Length;

		sbRouter.SendMessage(Message.ConstructMessage("MSG", "A " + length + "\r\n" + mimeHeader + text), new ResponseReceivedHandler(OnSayAck), opCompleteEvent, false);
	}


	public void InviteFriend(Friend friend, OperationCompleteEvent opCompleteEvent)
	{
		sbRouter.AddCodeEvent("JOI", new ResponseReceivedHandler(OnJoinAfterInvite), opCompleteEvent);
		sbRouter.SendMessage(Message.ConstructMessage("CAL", friend.Username), new ResponseReceivedHandler(OnInviteFriendComplete), opCompleteEvent);
	}


	public void SendTypingNotification(OperationCompleteEvent opCompleteEvent)
	{
		string body = "MIME-Version: 1.0\r\n"
			+ "Content-Type: text/x-msmsgscontrol\r\n"
			+ "TypingUser: " + this.settings.Username + "\r\n\r\n\r\n";

		this.sbRouter.SendMessage( Message.ConstructMessage("MSG", "U " + body.Length, body, true), false );

		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	public void SendFile(IO.Stream stream, OperationCompleteEvent opCompleteEvent)
	{



		opCompleteEvent.Execute(new OperationCompleteArgs());
	}
}
