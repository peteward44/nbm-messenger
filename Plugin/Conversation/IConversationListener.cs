using System;

// 12/6/03

namespace NBM.Plugin
{
	/// <summary>
	/// Classes that implement this interface receive events about conversations.
	/// This should not be used by plugin authors.
	/// </summary>
	public interface IConversationListener
	{
		/// <summary>
		/// Called when the conversation connects
		/// </summary>
		void OnConversationConnected();


		/// <summary>
		/// Called when the conversation disconnects
		/// </summary>
		void OnConversationDisconnected();


		/// <summary>
		/// Called when a friend says something in conversation
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="text">Message</param>
		void OnFriendSay(Friend friend, string text);


		/// <summary>
		/// Called when a friend joins a conversation
		/// </summary>
		/// <param name="friend"></param>
		void OnFriendJoin(Friend friend);


		/// <summary>
		/// Called when a friend leaves a conversation
		/// </summary>
		/// <param name="friend"></param>
		void OnFriendLeave(Friend friend);


		/// <summary>
		/// Called when a typing notification is received
		/// </summary>
		/// <param name="friend"></param>
		void OnTypingNotification(Friend friend);


		/// <summary>
		/// Called when the user says something
		/// </summary>
		/// <param name="text"></param>
		void OnUserSay(string text);


		/// <summary>
		/// Called when a file invitation is received
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="filename"></param>
		void OnFileSendInvitation(Friend friend, string filename);
	}
}
