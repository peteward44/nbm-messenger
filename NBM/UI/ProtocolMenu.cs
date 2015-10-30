using System;
using NBM.Plugin;
using WinForms = System.Windows.Forms;

// 8/5/03

namespace NBM
{
	/// <summary>
	/// Context menu for the protocols
	/// </summary>
	public class ProtocolMenu : WinForms.ContextMenu, IProtocolListener
	{
		private MainForm mainForm;
		private Protocol protocol;

		private StatusMenu statusMenu;
		private WinForms.MenuItem addFriendItem, debugReportItem, optionsItem, connectItem, disconnectItem;


		/// <summary>
		/// Constructs a ProtocolMenu
		/// </summary>
		/// <param name="mainForm"></param>
		/// <param name="protocol"></param>
		public ProtocolMenu(MainForm mainForm, Protocol protocol)
		{
			this.mainForm = mainForm;
			this.protocol = protocol;
			this.protocol.AddListener(this);

			// add connect and disconnect menu items
			this.statusMenu = new StatusMenu("Status", new EventHandler(OnStatusClick));

			foreach (StatusMenuItem item in this.statusMenu.MenuItems)
			{
				item.RadioCheck = true;
				item.Checked = item.Status == this.protocol.Settings.Status;
			}

			this.MenuItems.Add(statusMenu);
			this.MenuItems.Add("-");

			this.connectItem = new WinForms.MenuItem("Connect", new EventHandler(connectMenuItem_Click));
			this.MenuItems.Add(this.connectItem);

			this.disconnectItem = new WinForms.MenuItem("Disconnect", new EventHandler(disconnectMenuItem_Click));
			this.disconnectItem.Enabled = false;
			this.MenuItems.Add(this.disconnectItem);
			this.MenuItems.Add("-");

			this.optionsItem = new WinForms.MenuItem("Options", new EventHandler(optionsMenuItem_Click));
			this.MenuItems.Add(this.optionsItem);
			this.MenuItems.Add("-");

			this.addFriendItem = new WinForms.MenuItem("Add friend", new EventHandler(addfriendMenuItem_Click));
			this.addFriendItem.Enabled = false;
			this.MenuItems.Add(this.addFriendItem);

			this.MenuItems.Add("-");

			this.debugReportItem = new WinForms.MenuItem("Debug Report", new EventHandler(debugReportMenuItem_Click));
			this.MenuItems.Add(this.debugReportItem);

			this.Disposed += new EventHandler(OnDispose);

//			this.MenuItems.Add("Self Destruct", new EventHandler(selfdestruct_click));
		}


		private void OnStatusClick(object sender, EventArgs e)
		{
			this.protocol.ChangeStatus(((StatusMenuItem)sender).Status, new OperationCompleteEvent());
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnDispose(object sender, EventArgs e)
		{
			if (this.protocol != null)
				this.protocol.RemoveListener(this);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void connectMenuItem_Click(object sender, System.EventArgs e)
		{
			protocol.Connect(new OperationCompleteEvent());
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void disconnectMenuItem_Click(object sender, System.EventArgs e)
		{
			protocol.Disconnect(new OperationCompleteEvent());
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void addfriendMenuItem_Click(object sender, System.EventArgs e)
		{
			if (this.protocol.Connected)
			{
				// show add friend dialog
				AddFriendForm form = new AddFriendForm(this.protocol);
				if (form.ShowDialog() == WinForms.DialogResult.OK)
				{
					this.protocol.AddFriendToList(form.Username, form.Message, new OperationCompleteEvent());
				}
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void debugReportMenuItem_Click(object sender, System.EventArgs e)
		{
			this.protocol.Reporter.Show();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void optionsMenuItem_Click(object sender, System.EventArgs e)
		{
			OptionsForm form = new OptionsForm(this.mainForm, this.protocol);
			form.Show();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void selfdestruct_click(object sender, EventArgs e)
		{
			this.protocol.SelfDestruct();
		}


		#region ProtocolListener methods


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnFriendAdd(Friend friend)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnFriendRemove(Friend friend)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="status"></param>
		public void OnFriendChangeStatus(Friend friend, OnlineStatus status)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="newName"></param>
		public void OnFriendChangeDisplayName(Friend friend, string newName)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnBeginConnect()
		{
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnConnect()
		{
			this.addFriendItem.Enabled = true;
			this.connectItem.Enabled = false;
			this.disconnectItem.Enabled = true;
		}


		/// <summary>
		/// Called when the connection is canceled
		/// </summary>
		public void OnConnectCanceled()
		{
			this.addFriendItem.Enabled = false;
			this.disconnectItem.Enabled = false;
			this.connectItem.Enabled = true;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="forced"></param>
		public void OnDisconnect(bool forced)
		{
			this.addFriendItem.Enabled = false;
			this.disconnectItem.Enabled = false;
			this.connectItem.Enabled = true;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="status"></param>
		public void OnChangeStatus(OnlineStatus status)
		{
			foreach (StatusMenuItem item in this.statusMenu.MenuItems)
			{
				item.Checked = item.Status == status;
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
