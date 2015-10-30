using System;
using WinForms = System.Windows.Forms;
using Drawing = System.Drawing;

// 13/6/03

namespace NBM
{
	/// <summary>
	/// System tray icon.
	/// </summary>
	public class SysTrayIcon : IDisposable
	{
		private WinForms.NotifyIcon notify = new WinForms.NotifyIcon();
		private MainForm mainForm;


		/// <summary>
		/// Visible flag
		/// </summary>
		public bool Visible
		{
			get { return this.notify.Visible; }
			set { this.notify.Visible = value; }
		}


		/// <summary>
		/// Constructs a system tray icon
		/// </summary>
		/// <param name="mainForm"></param>
		/// <param name="icon"></param>
		public SysTrayIcon(MainForm mainForm, Drawing.Icon icon)
		{
			this.mainForm = mainForm;

			this.notify = new WinForms.NotifyIcon();
			this.notify.Text = "NBM";
			this.notify.Icon = icon;
			this.notify.Click += new EventHandler(OnSysTrayClick);
			this.notify.ContextMenu = new SysTrayContextMenu(mainForm);
			this.notify.Visible = true;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSysTrayClick(object sender, EventArgs e)
		{
			this.mainForm.Show();
			this.mainForm.WindowState = WinForms.FormWindowState.Normal;

			this.mainForm.Activate();
		}


		/// <summary>
		/// Destructor / Finalizer
		/// </summary>
		~SysTrayIcon()
		{
			this.Dispose();
		}


		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			if (notify != null)
				notify.Dispose();
			notify = null;

			System.GC.SuppressFinalize(this);
		}
	}
}
