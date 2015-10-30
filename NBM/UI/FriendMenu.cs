using System;
using WinForms = System.Windows.Forms;
using NBM.Plugin;

// 18/6/03

namespace NBM
{
	/// <summary>
	/// Context menu shown when you right-click on a friend's name.
	/// </summary>
	public class FriendMenu : WinForms.ContextMenu
	{
		private Protocol protocol;
		private Friend friend;

		private WinForms.MenuItem blockItem;


		/// <summary>
		/// Constructs a friend menu.
		/// </summary>
		/// <param name="protocol">Associated protocol</param>
		/// <param name="friend">Associated friend</param>
		public FriendMenu(Protocol protocol, Friend friend)
		{
			this.protocol = protocol;
			this.friend = friend;

			this.MenuItems.Add("Remove", new EventHandler(menuItemRemove_Click));

			this.blockItem = new WinForms.MenuItem();

			this.blockItem.Text = (friend.Blocked ? "Unblock" : "Block");
			this.blockItem.Click += new EventHandler(menuItemBlock_Click);

			this.MenuItems.Add(blockItem);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuItemRemove_Click(object sender, EventArgs e)
		{
			// confirm before removing
			if (WinForms.MessageBox.Show(
				"Are you sure you wish to remove " + friend.Username + " from your contact list?",
				"Remove Friend",
				WinForms.MessageBoxButtons.YesNo, WinForms.MessageBoxIcon.Hand) == WinForms.DialogResult.Yes)
			{
				this.protocol.RemoveFriendFromList(this.friend, new OperationCompleteEvent());
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuItemBlock_Click(object sender, EventArgs e)
		{
			if (friend.Blocked)
				this.protocol.UnblockFriend(this.friend, new OperationCompleteEvent());
			else
				this.protocol.BlockFriend(this.friend, new OperationCompleteEvent());

			friend.Blocked = !friend.Blocked;

			this.blockItem.Text = (friend.Blocked ? "Unblock" : "Block");
		}
	}
}
