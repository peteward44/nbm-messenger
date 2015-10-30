using System;
using System.Collections;

// 12/6/03

namespace NBM.Plugin
{
	/// <summary>
	/// This is passed to the implemented IConversation's constructor.
	/// Classes that implement IConversation use this class to raise events, e.g.
	/// like a friend speaking or another user joining the conversation.
	/// </summary>
	public class ConversationControl
	{
		private ConversationServer server;


		/// <summary>
		/// Constructs a ConversationControl.
		/// </summary>
		/// <param name="server">ConversationServer to pass events onto</param>
		public ConversationControl(ConversationServer server)
		{
			this.server = server;
		}


		/// <summary>
		/// Call when another user says something in the conversation
		/// </summary>
		/// <param name="username"></param>
		/// <param name="text"></param>
		public void FriendSay(string username, string text)
		{
			this.server.FriendSay(username, text);
		}


		/// <summary>
		/// Call when another user says something in the conversation
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="text"></param>
		public void FriendSay(Friend friend, string text)
		{
			this.server.FriendSay(friend, text);
		}


		/// <summary>
		/// Call when a someone joins the conversation
		/// </summary>
		/// <param name="username"></param>
		public void FriendJoin(string username)
		{
			this.server.FriendJoin(username);
		}


		/// <summary>
		/// Call when a friend joins the conversation
		/// </summary>
		/// <param name="friend"></param>
		public void FriendJoin(Friend friend)
		{
			this.server.FriendJoin(friend);
		}


		/// <summary>
		/// Call when a friend leaves the conversation
		/// </summary>
		/// <param name="username"></param>
		public void FriendLeave(string username)
		{
			this.server.FriendLeave(username);
		}

		
		/// <summary>
		/// Call when a friend leaves the conversation
		/// </summary>
		/// <param name="friend"></param>
		public void FriendLeave(Friend friend)
		{
			this.server.FriendLeave(friend);
		}


		/// <summary>
		/// Call when a friend is typing
		/// </summary>
		/// <param name="username">Name of friend typing</param>
		public void TypingNotification(string username)
		{
			this.server.TypingNotification(username);
		}


		/// <summary>
		/// Call when a friend is typing
		/// </summary>
		/// <param name="friend">Friend who is typing</param>
		public void TypingNotification(Friend friend)
		{
			this.server.TypingNotification(friend);
		}


		/// <summary>
		/// Creates a Proxy connection. Use this for all network connections.
		/// </summary>
		/// <returns></returns>
		public Proxy.IConnection CreateConnection()
		{
			return this.server.CreateConnection();
		}


		/// <summary>
		/// Call this when a file transfer invitation is received by someone else
		/// </summary>
		/// <param name="friend">Friend who's sending the file</param>
		/// <param name="filename">Filename</param>
		public void FileSendInvitation(Friend friend, string filename)
		{
			this.server.FileSendInvitation(friend, filename);
		}


		/// <summary>
		/// Call this when a file transfer invitation is received by someone else
		/// </summary>
		/// <param name="username">Friend who's sending the file</param>
		/// <param name="filename">Filename</param>
		public void FileSendInvitation(string username, string filename)
		{
			this.server.FileSendInvitation(username, filename);
		}
	}
}
