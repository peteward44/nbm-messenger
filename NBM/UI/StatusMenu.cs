using System;
using NBM.Plugin;
using WinForms = System.Windows.Forms;

// 13/5/03

namespace NBM
{
	/// <summary>
	/// Status menu item.
	/// </summary>
	public class StatusMenuItem : WinForms.MenuItem
	{
		private OnlineStatus status;


		/// <summary>
		/// Status which represents
		/// </summary>
		public OnlineStatus Status
		{
			get { return this.status; }
		}


		/// <summary>
		/// Constructs a status menu item
		/// </summary>
		/// <param name="status">Status to represent</param>
		/// <param name="e">Click event handler</param>
		public StatusMenuItem(OnlineStatus status, EventHandler e)
			: base(status.ToString(), e)
		{
			this.status = status;
		}
	}


	/// <summary>
	/// Context menu containing the status's available.
	/// </summary>
	public class StatusMenu : WinForms.MenuItem
	{
		/// <summary>
		/// Constructs a StatusMenu
		/// </summary>
		/// <param name="text">Text shown on menu option</param>
		/// <param name="statusClick">Event handler for when a menuitem is clicked</param>
		public StatusMenu(string text, EventHandler statusClick)
		{
			this.Text = text;

			this.MenuItems.Add(new StatusMenuItem(OnlineStatus.Online, statusClick));
			this.MenuItems.Add(new StatusMenuItem(OnlineStatus.Away, statusClick));
			this.MenuItems.Add(new StatusMenuItem(OnlineStatus.Busy, statusClick));
			this.MenuItems.Add(new StatusMenuItem(OnlineStatus.AppearOffline, statusClick));
		}
	}
}
