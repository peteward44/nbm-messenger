using System;
using IO = System.IO;

// 23/6/03

namespace NBM.Plugin
{
	/// <summary>
	/// All conversation plugin implementations implement this interface.
	/// </summary>
	public interface IConversation
	{
		/// <summary>
		/// Called when we want to start the conversation
		/// </summary>
		/// <param name="friend">Original friend who we want to talk to</param>
		/// <param name="opCompleteEvent"></param>
		void Start(Friend friend, OperationCompleteEvent opCompleteEvent);


		/// <summary>
		/// This is called soon after the plugin initiates a conversation through
		/// ProtocolControl.StartConversation()
		/// </summary>
		/// <param name="friend">Friend who initiated the conversation</param>
		/// <param name="opCompleteEvent"></param>
		/// <param name="tag">Plugin-defined object</param>
		void StartByInvitation(Friend friend, OperationCompleteEvent opCompleteEvent, object tag);


		/// <summary>
		/// Called when the conversation ends.
		/// </summary>
		/// <param name="opCompleteEvent"></param>
		void End(OperationCompleteEvent opCompleteEvent);


		/// <summary>
		/// Called when the user says something.
		/// </summary>
		/// <param name="text">Message to say</param>
		/// <param name="opCompleteEvent"></param>
		void Say(string text, OperationCompleteEvent opCompleteEvent);


		/// <summary>
		/// Called when the user wants to invite a friend to the conversation.
		/// </summary>
		/// <param name="friend">Friend to invite</param>
		/// <param name="opCompleteEvent"></param>
		void InviteFriend(Friend friend, OperationCompleteEvent opCompleteEvent);


		/// <summary>
		/// Called when the user is typing. This is not called if typing notifications are turned
		/// off in the options
		/// </summary>
		/// <param name="opCompleteEvent"></param>
		void SendTypingNotification(OperationCompleteEvent opCompleteEvent);


		/// <summary>
		/// Called when the user wants to send a file.
		/// </summary>
		/// <param name="stream">Data stream to send</param>
		/// <param name="opCompleteEvent"></param>
		void SendFile(IO.Stream stream, OperationCompleteEvent opCompleteEvent);
	}
}
