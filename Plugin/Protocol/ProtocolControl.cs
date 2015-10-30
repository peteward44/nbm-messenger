using System;
using System.Collections;

// 1/6/03

namespace NBM.Plugin
{
	/// <summary>
	/// This is passed to the implemented IProtocol's constructor.
	/// Classes that implement IProtocol use this class to raise events, e.g.
	/// like a friend coming online or a chat session initiated.
	/// </summary>
	public class ProtocolControl
	{
		private ProtocolServer server;


		/// <summary>
		/// Constructs a ProtocolControl.
		/// </summary>
		/// <param name="server">ProtocolServer to relay events onto</param>
		public ProtocolControl(ProtocolServer server)
		{
			this.server = server;
		}


		/// <summary>
		/// Creates a proxy connection. Use this for all network connections.
		/// </summary>
		/// <returns></returns>
		public Proxy.IConnection CreateConnection()
		{
			return this.server.CreateConnection();
		}


		/// <summary>
		/// Call this when the server disconnects the plugin.
		/// </summary>
		public void ForcedDisconnect()
		{
			this.server.ForcedDisconnect();
		}


		/// <summary>
		/// Call this when the plugin receives an invitation to a conversation
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="opCompleteEvent"></param>
		/// <param name="tag">Plugin defined object</param>
		public void StartInvitedConversation(Friend friend, OperationCompleteEvent opCompleteEvent, object tag)
		{
			this.server.StartInvitedConversation(friend, opCompleteEvent, tag);
		}


		/// <summary>
		/// Adds a friend to the contact list
		/// </summary>
		/// <param name="friend"></param>
		public void AddFriend(Friend friend)
		{
			this.server.AddFriend(friend);
		}


		/// <summary>
		/// Removes a friend from the contact list
		/// </summary>
		/// <param name="friend"></param>
		public void RemoveFriend(Friend friend)
		{
			this.server.RemoveFriend(friend);
		}


		/// <summary>
		/// Removes a friend from the contact list
		/// </summary>
		/// <param name="username"></param>
		public void RemoveFriend(string username)
		{
			this.server.RemoveFriend(username);
		}


		/// <summary>
		/// Checks if the contact list already contains a friend
		/// </summary>
		/// <param name="friend"></param>
		/// <returns></returns>
		public bool ContainsFriend(Friend friend)
		{
			return this.server.ContainsFriend(friend);
		}


		/// <summary>
		/// Checks if the contact list already contains a friend
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public bool ContainsFriend(string username)
		{
			return this.server.ContainsFriend(username);
		}


		/// <summary>
		/// Gets said friend from the contact list
		/// </summary>
		/// <param name="username">Username of friend</param>
		/// <returns>null if friend is not found</returns>
		public Friend GetFriend(string username)
		{
			return this.server.GetFriend(username);
		}


		/// <summary>
		/// Call this when the plugin wishes to write to the debug log
		/// </summary>
		/// <param name="text"></param>
		public void WriteDebug(string text)
		{
			this.server.WriteDebug(text);
		}


		/// <summary>
		/// Call this when the plugin wishes to write to the debug log
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void WriteDebug(string format, params object[] args)
		{
			this.server.WriteDebug(format, args);
		}


		/// <summary>
		/// Call this when the plugin wishes to ask the user what to do when a stranger has
		/// added the user to their contact list
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="reason"></param>
		/// <param name="enableAddCheckbox"></param>
		public void PromptForStrangerHasAddedMe(Friend friend, string reason, bool enableAddCheckbox)
		{
			this.server.PromptForStrangerHasAddedMe(friend, reason, enableAddCheckbox);
		}
	}
}
