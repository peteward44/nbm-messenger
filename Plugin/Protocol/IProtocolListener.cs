using System;
using System.Collections;

// 1/6/03

namespace NBM.Plugin
{
	/// <summary>
	/// Available status's
	/// </summary>
	public enum OnlineStatus
	{
		/// <summary>
		/// 
		/// </summary>
		Online,
		/// <summary>
		/// 
		/// </summary>
		Away,
		/// <summary>
		/// 
		/// </summary>
		Idle,
		/// <summary>
		/// 
		/// </summary>
		Busy,
		/// <summary>
		/// 
		/// </summary>
		AppearOffline,
		/// <summary>
		/// 
		/// </summary>
		Offline
	}


	/// <summary>
	/// Classes that implement this interface receive events about the protocol.
	/// This should not be used by plugin authors.
	/// </summary>
	public interface IProtocolListener
	{
		/// <summary>
		/// Called when a friend is added to the contact list
		/// </summary>
		/// <param name="friend"></param>
		void OnFriendAdd(Friend friend);

		
		/// <summary>
		/// Called when a friend is removed from the contact list
		/// </summary>
		/// <param name="friend"></param>
		void OnFriendRemove(Friend friend);


		/// <summary>
		/// Called when a friend changes their status
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="status"></param>
		void OnFriendChangeStatus(Friend friend, OnlineStatus status);


		/// <summary>
		/// Called when a friend changes their display name
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="newName"></param>
		void OnFriendChangeDisplayName(Friend friend, string newName);


		/// <summary>
		/// Called when a connection has started
		/// </summary>
		void OnBeginConnect();


		/// <summary>
		/// Called when a connection has completed
		/// </summary>
		void OnConnect();


		/// <summary>
		/// When the connection is canceled
		/// </summary>
		void OnConnectCanceled();


		/// <summary>
		/// Called when the protocol is disconnected
		/// </summary>
		/// <param name="forced">True if the disconnection was unexpected, false if the user
		/// requested the disconnect</param>
		void OnDisconnect(bool forced);


		/// <summary>
		/// Called when the user changes his status
		/// </summary>
		/// <param name="status"></param>
		void OnChangeStatus(OnlineStatus status);


		/// <summary>
		/// Called when the user has been invited to a conversation by someone else
		/// </summary>
		/// <param name="friend">Friend who's asked for a conversation</param>
		/// <param name="opCompleteEvent"></param>
		/// <param name="tag">Plugin-defined object</param>
		void OnInvitedToConversation(Friend friend, OperationCompleteEvent opCompleteEvent, object tag);


		/// <summary>
		/// Called when the user has added someone to their contact list
		/// </summary>
		/// <param name="username"></param>
		void OnAddFriendToList(string username);


		/// <summary>
		/// Called when the user has removed someone from their contact list
		/// </summary>
		/// <param name="friend"></param>
		void OnRemoveFriendFromList(Friend friend);


		/// <summary>
		/// Called when the user blocks someone on their contact list
		/// </summary>
		/// <param name="friend"></param>
		void OnBlockFriend(Friend friend);


		/// <summary>
		/// Called when the user unblocks someone on their contact list
		/// </summary>
		/// <param name="friend"></param>
		void OnUnblockFriend(Friend friend);


		/// <summary>
		/// Called when the plugin wishes to write something to the protocol's debug log
		/// </summary>
		/// <param name="text"></param>
		void OnWriteDebug(string text);


		/// <summary>
		/// Called by the plugin when a stranger has added the user to their contact list
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="reason"></param>
		/// <param name="enableAddCheckbox"></param>
		void OnPromptForStrangerHasAddedMe(Friend friend, string reason, bool enableAddCheckbox);
	}
}
