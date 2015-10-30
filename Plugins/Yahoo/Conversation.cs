using System;
using NBM.Plugin;
using IO = System.IO;

// 15/5/03

public class Conversation : IConversation
{
	private Protocol protocol;
	internal ConversationControl control;
	private Settings settings;
	private Friend friend;


	public Conversation(IProtocol protocol, ConversationControl control, ProtocolSettings settings)
	{
		this.protocol = (Protocol)protocol;
		this.control = control;
		this.settings = (Settings)settings;
	}


	public void Start(Friend friend, OperationCompleteEvent opCompleteEvent)
	{
		this.friend = friend;
		this.protocol.conversationTable.Add(friend.Username, this);
		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	public void StartByInvitation(Friend friend, OperationCompleteEvent opCompleteEvent, object tag)
	{
		this.friend = friend;
		opCompleteEvent.Execute(new OperationCompleteArgs());
		Message message = (Message)tag;
		this.control.FriendSay(friend, message.GetArgumentString(14));

		this.protocol.conversationTable.Add(friend.Username, this);
	}


	public void End(OperationCompleteEvent opCompleteEvent)
	{
		this.protocol.conversationTable.Remove(friend.Username);
		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	public void Say(string text, OperationCompleteEvent opCompleteEvent)
	{
		protocol.Say(friend.Username, text, opCompleteEvent);
	}


	public void InviteFriend(Friend friend, OperationCompleteEvent opCompleteEvent)
	{
		opCompleteEvent.Execute(new OperationCompleteArgs());
	}


	internal void RouteMessage(Message message)
	{
		string friendname = message.GetArgumentString(4).ToLower();
		string text = message.GetArgumentString(14);

		if (friendname != null && text != null)
			this.control.FriendSay(friendname, text);
	}


	public void SendTypingNotification(OperationCompleteEvent opCompleteEvent)
	{
		protocol.SendTypingNotification(this.friend, opCompleteEvent);
	}


	public void SendFile(IO.Stream stream, OperationCompleteEvent opCompleteEvent)
	{
		opCompleteEvent.Execute(new OperationCompleteArgs());
	}
}
