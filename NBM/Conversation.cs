using System;
using NBM.Plugin;

// 2/6/03

namespace NBM
{
	/// <summary>
	/// Represents a conversation.
	/// </summary>
	public class Conversation : IConversationListener
	{
		private MessageForm messageForm;
		private Protocol protocol;
		private ConversationControl conversationControl = null;


		public Conversation(MessageForm messageForm, Protocol protocol)
		{
			this.messageForm = messageForm;
			this.protocol = protocol;
		}


		public void OnFriendSay(Friend friend, string text)
		{
		}


		public void OnFriendJoin(Friend friend)
		{
		}


		public void OnFriendLeave(Friend friend)
		{
		}
	}
}
