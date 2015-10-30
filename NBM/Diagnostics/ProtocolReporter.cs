using System;

using NBM.Plugin;

// 28/6/03

namespace NBM.Diagnostics
{
	/// <summary>
	/// Summary description for ProtocolReporter.
	/// </summary>
	public class ProtocolReporter : IProtocolListener, IDisposable
	{
		private Protocol protocol;
		private Debug debug;


		/// <summary>
		/// Whether the debug window is visible or not
		/// </summary>
		public bool Visible
		{
			get { return debug.Visible; }
			set { debug.Visible = value; }
		}


		/// <summary>
		/// Constructs a ProtocolReporter
		/// </summary>
		/// <param name="protocol">Protocol to attach to</param>
		public ProtocolReporter(Protocol protocol)
		{
			this.protocol = protocol;
			this.debug = new Debug(protocol.Name);

			this.protocol.AddListener(this);
		}


		/// <summary>
		/// Show the debug window
		/// </summary>
		public void Show()
		{
			this.debug.Show();
		}


		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			this.debug.Dispose();
		}


		/// <summary>
		/// Hide the debug window
		/// </summary>
		public void Hide()
		{
			this.debug.Hide();
		}


		#region ProtocolListener methods


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnFriendAdd(Friend friend)
		{
			this.debug.WriteLine("Contact added: " + friend.Username);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnFriendRemove(Friend friend)
		{
			this.debug.WriteLine("Contact removed: " + friend.Username);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="status"></param>
		public void OnFriendChangeStatus(Friend friend, OnlineStatus status)
		{
			this.debug.WriteLine("Contact changed status: " + friend.Username);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="newName"></param>
		public void OnFriendChangeDisplayName(Friend friend, string newName)
		{
			this.debug.WriteLine("Contact changed display name: " + friend.Username);
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnBeginConnect()
		{
			this.debug.WriteLine("Connection begun");
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnConnect()
		{
			this.debug.WriteLine("Connection completed");
		}


		/// <summary>
		/// Called when the connection is canceled
		/// </summary>
		public void OnConnectCanceled()
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="forced"></param>
		public void OnDisconnect(bool forced)
		{
			this.debug.WriteLine("Disconnected, forced : " + forced.ToString());
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="status"></param>
		public void OnChangeStatus(OnlineStatus status)
		{
			this.debug.WriteLine("User changed status");
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="opCompleteEvent"></param>
		/// <param name="tag"></param>
		public void OnInvitedToConversation(Friend friend, OperationCompleteEvent opCompleteEvent, object tag)
		{
			this.debug.WriteLine("Conversation invite received from " + friend.Username);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"></param>
		public void OnAddFriendToList(string username)
		{
			this.debug.WriteLine("Contact added to list: " + username);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnRemoveFriendFromList(Friend friend)
		{
			this.debug.WriteLine("Contact removed from list: " + friend.Username);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnBlockFriend(Friend friend)
		{
			this.debug.WriteLine("Contact blocked: " + friend.Username);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnUnblockFriend(Friend friend)
		{
			this.debug.WriteLine("Contact unblocked: " + friend.Username);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		public void OnWriteDebug(string text)
		{
			this.debug.WriteLine(text);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="reason"></param>
		/// <param name="enableAddCheckbox"></param>
		public void OnPromptForStrangerHasAddedMe(Friend friend, string reason, bool enableAddCheckbox)
		{
		}


		#endregion


	}
}
