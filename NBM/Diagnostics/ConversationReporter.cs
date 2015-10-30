using System;

using NBM.Plugin;


// 28/6/03

namespace NBM.Diagnostics
{
	/// <summary>
	/// Debug logging for conversations.
	/// </summary>
	public class ConversationReporter : IConversationListener
	{
		/// <summary>
		/// 
		/// </summary>
		public ConversationReporter()
		{
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnConversationConnected()
		{
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnConversationDisconnected()
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="text"></param>
		public void OnFriendSay(Friend friend, string text)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnFriendJoin(Friend friend)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnFriendLeave(Friend friend)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnTypingNotification(Friend friend)
		{
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
		/// <param name="filename"></param>
		public void OnFileSendInvitation(Friend friend, string filename)
		{
		}
	}
}
