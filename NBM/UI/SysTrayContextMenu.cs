using System;
using NBM.Plugin;
using NBM.Plugin.Settings;
using WinForms = System.Windows.Forms;


// 13/6/03

namespace NBM
{
	/// <summary>
	/// Context menu for the system tray icon.
	/// </summary>
	public class SysTrayContextMenu : WinForms.ContextMenu
	{
		private MainForm mainForm;
		private StatusMenu statusMenu;

		
		/// <summary>
		/// Constructs a SysTrayContextMenu
		/// </summary>
		/// <param name="mainForm"></param>
		public SysTrayContextMenu(MainForm mainForm)
		{
			this.mainForm = mainForm;

			this.statusMenu = new StatusMenu("Global Status", new EventHandler(OnStatusClick));
			
			this.MenuItems.Add(this.statusMenu);
			this.MenuItems.Add("-");
			this.MenuItems.Add("Connect All", new EventHandler(OnConnectAll));
			this.MenuItems.Add("Disconnect All", new EventHandler(OnDisconnectAll));
			this.MenuItems.Add("-");
			this.MenuItems.Add("Options", new EventHandler(OnOptions));
			this.MenuItems.Add("-");
			this.MenuItems.Add("Exit", new EventHandler(OnExit));
		}


		/// <summary>
		/// When the user changes the status via the status menu item
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnStatusClick(object sender, EventArgs e)
		{
			mainForm.ChangeGlobalStatus(((StatusMenuItem)sender).Status);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnOptions(object sender, EventArgs args)
		{
			OptionsForm options = new OptionsForm(mainForm);
			options.ShowDialog();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnExit(object sender, EventArgs args)
		{
			MainForm.Exit();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnConnectAll(object sender, EventArgs args)
		{
			this.mainForm.ConnectAll();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		private void OnDisconnectAll(object sender, EventArgs args)
		{
			this.mainForm.DisconnectAll();
		}
	}
}
