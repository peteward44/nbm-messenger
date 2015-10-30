using System;
using System.Collections;
using WinForms = System.Windows.Forms;
using Drawing = System.Drawing;
using NBM.Plugin;

// 4/6/03

namespace NBM
{
	/// <summary>
	/// Describes a tree node protocol for the contact tree view.
	/// </summary>
	public class ProtocolTreeNode : TreeNodeEx, IProtocolListener
	{
		private Protocol protocol;
		private OfflineTreeNode offlineTreeNode;
		private Hashtable friendsTable = new Hashtable();
		private int protocolIndex = 0;


		/// <summary>
		/// Associated protocol
		/// </summary>
		public Protocol Protocol
		{
			get { return protocol; }
		}


		/// <summary>
		/// Constructs a ProtocolTreeNode
		/// </summary>
		/// <param name="protocol"></param>
		/// <param name="offlineTreeNode"></param>
		/// <param name="protocolIndex">Each protocol is assigned a number, for image indexing purposes. This is that number.</param>
		public ProtocolTreeNode(Protocol protocol, OfflineTreeNode offlineTreeNode, int protocolIndex)
			: base(protocol.Name)
		{
			this.protocol = protocol;
			this.offlineTreeNode = offlineTreeNode;
			this.protocolIndex = protocolIndex;

			this.protocol.AddListener(this);

			SetProtocolNodeImageIndex(protocol.Settings.Status);
		}


		/// <summary>
		/// Calculates the image index appropriate for the friend's status icon.
		/// </summary>
		/// <param name="status"></param>
		/// <returns></returns>
		private int GetFriendNodeImageIndex(OnlineStatus status)
		{
			switch (status)
			{
				default:
				case OnlineStatus.Online:
					return 0;

				case OnlineStatus.Idle:
				case OnlineStatus.Away:
					return 1;

				case OnlineStatus.Busy:
					return 2;

				case OnlineStatus.AppearOffline:
				case OnlineStatus.Offline:
					return 3 + ((protocolIndex+1) * 9);
			}
		}


		/// <summary>
		/// Sets the protocol image index for the user status.
		/// </summary>
		/// <param name="status"></param>
		private void SetProtocolNodeImageIndex(OnlineStatus status)
		{
			int imageIndex = 3 + (protocolIndex * 9);

			switch (status)
			{
				default:
				case OnlineStatus.Online:
					imageIndex += 1;
					break;

				case OnlineStatus.Idle:
				case OnlineStatus.Away:
					imageIndex += 2;
					break;

				case OnlineStatus.Busy:
					imageIndex += 3;
					break;

				case OnlineStatus.AppearOffline:
				case OnlineStatus.Offline:
					imageIndex += 4;
					break;
			}

			this.ExpandedImageIndex = imageIndex;
			this.CollapsedImageIndex = imageIndex + 4;
		}


		/// <summary>
		/// Sets the tooltip for the apporpriate node.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="displayName"></param>
		/// <param name="status"></param>
		/// <param name="email"></param>
		private void SetFriendToolTip(TreeNodeEx node, string displayName, OnlineStatus status, string email)
		{
			// change tool tip text
			node.ToolTipText = "Name: " + displayName + "\nStatus: " + status.ToString() + "\nEmail: " + email;
		}


		#region ProtocolListener events


		#region Invoke methods


		private delegate void FriendTreeHandler(TreeNodeEx parent, TreeNodeEx child);


		private static void AddFriendToTree(TreeNodeEx parent, TreeNodeEx child)
		{
			parent.Nodes.Add(child);
		}


		private static void RemoveFriendFromTree(TreeNodeEx parent, TreeNodeEx child)
		{
			parent.Nodes.Remove(child);
		}


		#endregion


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnFriendAdd(Friend friend)
		{
			FriendTreeNode node = new FriendTreeNode(this.protocol, friend);

			node.ContextMenu = new FriendMenu(this.protocol, friend);
			node.CollapsedImageIndex = node.ExpandedImageIndex = this.GetFriendNodeImageIndex(friend.Status);

			SetFriendToolTip(node, friend.DisplayName, friend.Status, friend.EmailAddress);

			if (friend.Status == OnlineStatus.Offline)
				offlineTreeNode.Nodes.Add(node);
			else
				this.Nodes.Add(node);

			this.friendsTable.Add(friend, node);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnFriendRemove(Friend friend)
		{
			FriendTreeNode node = (FriendTreeNode)this.friendsTable[friend];
			node.Remove();
			this.friendsTable.Remove(friend);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="newStatus"></param>
		public void OnFriendChangeStatus(Friend friend, OnlineStatus newStatus)
		{
			FriendTreeNode node = (FriendTreeNode)this.friendsTable[friend];

			if (node != null)
			{
				// friend went offline, transfer him to the offline list
				if (friend.Status != OnlineStatus.Offline && newStatus == OnlineStatus.Offline)
				{
					// move treenode to offline tree
					this.TreeView.Invoke(new FriendTreeHandler(RemoveFriendFromTree), new object[] { this, node });
					this.TreeView.Invoke(new FriendTreeHandler(AddFriendToTree), new object[] { this.offlineTreeNode, node });
				}
					// else friend has just came online from being offline
				else if (friend.Status == OnlineStatus.Offline && newStatus != OnlineStatus.Offline)
				{
					// move treenode from offline tree to protocol tree
					this.TreeView.Invoke(new FriendTreeHandler(RemoveFriendFromTree), new object[] { this.offlineTreeNode, node });
					this.TreeView.Invoke(new FriendTreeHandler(AddFriendToTree), new object[] { this, node });
				}

				node.CollapsedImageIndex = node.ExpandedImageIndex = GetFriendNodeImageIndex(newStatus);
				SetFriendToolTip(node, friend.DisplayName, newStatus, friend.EmailAddress);
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="newName"></param>
		public void OnFriendChangeDisplayName(Friend friend, string newName)
		{
			FriendTreeNode node = (FriendTreeNode)this.friendsTable[friend];
			node.Text = newName;
			SetFriendToolTip(node, newName, friend.Status, friend.EmailAddress);
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnBeginConnect()
		{
			this.FontStyle = Drawing.FontStyle.Italic;
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnConnect()
		{
			this.FontStyle = Drawing.FontStyle.Bold;
		}


		/// <summary>
		/// Called when the connection is canceled
		/// </summary>
		public void OnConnectCanceled()
		{
			this.FontStyle = Drawing.FontStyle.Regular;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="forced"></param>
		public void OnDisconnect(bool forced)
		{
			this.FontStyle = Drawing.FontStyle.Regular;
			this.friendsTable.Clear();

			this.Nodes.Clear();

			for (int i=0; i<this.offlineTreeNode.Nodes.Count; ++i)
			{
				FriendTreeNode node = (FriendTreeNode)this.offlineTreeNode.Nodes[i];

				if (node.Protocol.Name == this.protocol.Name)
				{
					node.Remove();
					--i;
				}
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="opCompleteEvent"></param>
		/// <param name="tag"></param>
		public void OnInvitedToConversation(Friend friend, OperationCompleteEvent opCompleteEvent, object tag)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="status"></param>
		public void OnChangeStatus(OnlineStatus status)
		{
			SetProtocolNodeImageIndex(status);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"></param>
		public void OnAddFriendToList(string username)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnRemoveFriendFromList(Friend friend)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnBlockFriend(Friend friend)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnUnblockFriend(Friend friend)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		public void OnWriteDebug(string text)
		{
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
