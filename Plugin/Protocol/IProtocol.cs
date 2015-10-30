using System;

namespace NBM.Plugin
{
	/// <summary>
	/// All protocol implementations implement this interface.
	/// </summary>
	public interface IProtocol
	{
		/// <summary>
		/// Implemented by plugins to connect
		/// </summary>
		/// <param name="opCompleteEvent"></param>
		void Connect(OperationCompleteEvent opCompleteEvent);


		/// <summary>
		/// Implemented by plugins to disconnect
		/// </summary>
		/// <param name="opCompleteEvent"></param>
		void Disconnect(OperationCompleteEvent opCompleteEvent);


		/// <summary>
		/// Implemented by plugins to cancel the current connection
		/// </summary>
		void CancelConnection(OperationCompleteEvent opCompleteEvent);


		/// <summary>
		/// Implemented by plugins to change the user's status
		/// </summary>
		/// <param name="status">Status to change to</param>
		/// <param name="opCompleteEvent"></param>
		void ChangeStatus(OnlineStatus status, OperationCompleteEvent opCompleteEvent);


		/// <summary>
		/// Implemented by plugins to block a friend on the contact list
		/// </summary>
		/// <param name="friend">Friend to block</param>
		/// <param name="opCompleteEvent"></param>
		void BlockFriend(Friend friend, OperationCompleteEvent opCompleteEvent);


		/// <summary>
		/// Implemented by plugins to unblock a friend on the contact list
		/// </summary>
		/// <param name="friend">Friend to block</param>
		/// <param name="opCompleteEvent"></param>
		void UnblockFriend(Friend friend, OperationCompleteEvent opCompleteEvent);


		/// <summary>
		/// Implemented by plugins to add a friend to the contact list
		/// </summary>
		/// <param name="username">Username to add</param>
		/// <param name="message">[optional] Message to send to friend</param>
		/// <param name="opCompleteEvent"></param>
		void AddFriendToList(string username, string message, OperationCompleteEvent opCompleteEvent);


		/// <summary>
		/// Implemented by plugins to remove a friend from the contact list
		/// </summary>
		/// <param name="friend">Friend on list to remove</param>
		/// <param name="opCompleteEvent"></param>
		void RemoveFriendFromList(Friend friend, OperationCompleteEvent opCompleteEvent);


		/// <summary>
		/// 
		/// </summary>
		void SelfDestruct();


		/// <summary>
		/// Shortly after ProtocolControl.PromptForStrangerHasAddedMe() is called this will be called,
		/// after asking the user what to do about when a stranger has added the user to their contact
		/// list.
		/// </summary>
		/// <param name="friend">Stranger who has added the user</param>
		/// <param name="message">[optional] Message to be sent by user to stranger</param>
		/// <param name="allowUser">True to allow the stranger to see the user's online status</param>
		/// <param name="addUser">True to add the stranger to the user's contact list</param>
		void PromptForStrangerHasAddedMeResponse(Friend friend, string message, bool allowUser, bool addUser);
	}
}