using System;
using System.Collections;
using WinForms = System.Windows.Forms;
using NBM.Plugin;

// 4/6/03

namespace NBM
{
	/// <summary>
	/// Tree node for ContactTreeView for friends.
	/// </summary>
	public class FriendTreeNode : TreeNodeEx
	{
		private Protocol protocol;
		private Friend friend;


		/// <summary>
		/// Associated friend
		/// </summary>
		public Friend Friend
		{
			get { return friend; }
		}


		/// <summary>
		/// Associated protocol
		/// </summary>
		public Protocol Protocol
		{
			get { return protocol; }
		}


		/// <summary>
		/// Constructs a FriendTreeNode
		/// </summary>
		/// <param name="protocol">Associated protocol</param>
		/// <param name="friend">Associated friend</param>
		public FriendTreeNode(Protocol protocol, Friend friend)
		{
			this.protocol = protocol;
			this.friend = friend;
			this.Tag = friend;
			this.Text = friend.DisplayName;
		}
	}
}
