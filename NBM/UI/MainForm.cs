using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Data;

using NBM.Plugin;
using NBM.Plugin.Settings;
using WinForms = System.Windows.Forms;
using Drawing = System.Drawing;
using IO = System.IO;

// 31/5/03

namespace NBM
{
	/// <summary>
	/// MainForm for the application.
	/// Holds the contact list etc.
	/// </summary>
	public class MainForm : WinForms.Form
	{
		private ContactTreeView contactTreeView;
		private IStorage mainStorage;
		private SysTrayIcon sysTrayIcon;
		private TaskbarNotifier taskbarNotify;

		private ArrayList protocolList = new ArrayList();


		/// <summary>
		/// List of protocols used in the application
		/// Note - this also contains protocols which the user may not have enabled,
		/// so check Protocol.Enabled first, k?
		/// </summary>
		public ArrayList Protocols
		{
			get { return protocolList; }
		}


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;


		/// <summary>
		/// Constructs a MainForm object. This is where most of the fun happens.
		/// </summary>
		/// <param name="mainStorage"></param>
		public MainForm(IStorage mainStorage)
		{
			this.mainStorage = mainStorage;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.MaximizeBox = false;

			// create sys tray icon
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.sysTrayIcon = new SysTrayIcon(this, (Drawing.Icon)resources.GetObject("$this.Icon"));

			// initialise task bar notifier
			this.taskbarNotify = new TaskbarNotifier();

			this.taskbarNotify.SetBackgroundBitmap(
				Config.Constants.ImagePath + System.IO.Path.DirectorySeparatorChar + "skin.bmp",
				Drawing.Color.FromArgb(255, 0, 255));

			this.taskbarNotify.TitleRectangle = new Drawing.Rectangle(70, 9, 70, 25);
			this.taskbarNotify.ContentRectangle = new Drawing.Rectangle(8, 41, 133, 68);

			this.taskbarNotify.NormalTitleColor = Drawing.Color.Red;
			this.taskbarNotify.NormalContentColor = Drawing.Color.DarkBlue;

			// load plugins (duh)
			LoadPlugins();

			// hook contact tree events
			this.contactTreeView.DoubleClick += new EventHandler(OnContactTreeDoubleClick);

			this.UpdateOptions();

			if (GlobalSettings.Instance().ConnectAllOnStartup)
				this.ConnectAll();
		}


		/// <summary>
		/// Updates the options. When a user changes the options through the options form,
		/// this ensures anything he's changes takes effect immediately.
		/// </summary>
		public void UpdateOptions()
		{
			NBM.Plugin.Settings.GlobalSettings globalSettings = NBM.Plugin.Settings.GlobalSettings.Instance();

			// add offline treenode
			if (globalSettings.HideOfflineContacts)
				this.contactTreeView.HideOfflineList();
			else
				this.contactTreeView.ShowOfflineList();

			this.ShowInTaskbar = globalSettings.DisplayTaskBar;
			this.TopMost = globalSettings.AlwaysOnTop;
			this.sysTrayIcon.Visible = true;


			foreach (Protocol protocol in this.protocolList)
			{
				ProtocolTreeNode node = this.contactTreeView.GetProtocolNode(protocol);
				if (node != null)
				{
					node.Visible = protocol.Settings.Enabled;
				}
			}
		}


		/// <summary>
		/// Changes the user's global status
		/// </summary>
		/// <param name="status"></param>
		public void ChangeGlobalStatus(OnlineStatus status)
		{
			foreach (Protocol protocol in this.protocolList)
			{
				if (protocol.Enabled)
					protocol.ChangeStatus(status, new OperationCompleteEvent());
			}
		}


		/// <summary>
		/// Connects all the protocols
		/// </summary>
		public void ConnectAll()
		{
			foreach (Protocol protocol in this.protocolList)
			{
				if (protocol.Enabled)
					protocol.Connect(new OperationCompleteEvent());
			}
		}


		/// <summary>
		/// Disconnects all protocols
		/// </summary>
		public void DisconnectAll()
		{
			foreach (Protocol protocol in this.protocolList)
			{
				if (protocol.Enabled)
					protocol.Disconnect(new OperationCompleteEvent());
			}
		}


		/// <summary>
		/// Override createparams so we can set the initial size and position of the form.
		/// </summary>
		protected override WinForms.CreateParams CreateParams
		{
			get
			{
				NBM.Plugin.Settings.GlobalSettings globalSettings = NBM.Plugin.Settings.GlobalSettings.Instance();

				// set location and size of contact list
				WinForms.CreateParams cp = base.CreateParams;

				cp.Width = globalSettings.ContactListSize.Width;
				cp.Height = globalSettings.ContactListSize.Height;
				cp.X = globalSettings.ContactListLocation.X;
				cp.Y = globalSettings.ContactListLocation.Y;

				return cp;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnLocationChanged(System.EventArgs e)
		{
			NBM.Plugin.Settings.GlobalSettings.Instance().ContactListLocation = this.Location;
			base.OnLocationChanged(e);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged(System.EventArgs e)
		{
			NBM.Plugin.Settings.GlobalSettings.Instance().ContactListSize = this.Size;
			base.OnSizeChanged(e);
		}


		/// <summary>
		/// Loads all protocols
		/// </summary>
		private void LoadPlugins()
		{
			// find all plugin assemblies and load them
			// open plugins directory
			IO.DirectoryInfo pluginDirectory = new IO.DirectoryInfo(Config.Constants.PluginsPath);

			if (!pluginDirectory.Exists)
				pluginDirectory.Create();

			// iterate through the files in there and load each one.
			IO.FileInfo[] fileInfoList = pluginDirectory.GetFiles();

			foreach (IO.FileInfo fileInfo in fileInfoList)
			{
				if (fileInfo.Extension.ToLower() == ".dll"
					&& fileInfo.Name.ToLower() != "proxy.dll"
					&& fileInfo.Name.ToLower() != "plugin.dll")
				{
					try
					{
						Protocol protocol = new Protocol(this.contactTreeView, this.taskbarNotify, fileInfo.FullName, this.mainStorage);
						this.protocolList.Add(protocol);
					}
					catch
					{}
				}
			}

			this.contactTreeView.AddProtocols(this.protocolList);
		}


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}

				foreach (Protocol protocol in this.protocolList)
				{
					protocol.Disconnect(new OperationCompleteEvent());
					protocol.Dispose();
				}

				if (this.sysTrayIcon != null)
					this.sysTrayIcon.Dispose();
				this.sysTrayIcon = null;

				if (this.taskbarNotify != null)
					this.taskbarNotify.Dispose();
				this.taskbarNotify = null;
			}
			base.Dispose( disposing );
		}


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.contactTreeView = new NBM.ContactTreeView(this);
			this.SuspendLayout();
			// 
			// contactTreeView
			// 
			this.contactTreeView.AllowDrop = true;
			this.contactTreeView.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right);
			this.contactTreeView.BackColor = System.Drawing.Color.White;
			this.contactTreeView.HeightIndent = 2;
			this.contactTreeView.Name = "contactTreeView";
			this.contactTreeView.Size = new System.Drawing.Size(144, 464);
			this.contactTreeView.TabIndex = 0;
			this.contactTreeView.WidthIndent = 5;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(144, 462);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																																	this.contactTreeView});
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "NBM";
			this.ResumeLayout(false);

		}
		#endregion


		/// <summary>
		/// Single exit point for the application.
		/// </summary>
		public static void Exit()
		{
			if (GlobalSettings.Instance().PromptOnExit)
			{
				if (WinForms.MessageBox.Show("Are you sure you wish to exit NBM?", "",
					WinForms.MessageBoxButtons.YesNo,
					WinForms.MessageBoxIcon.Question) == WinForms.DialogResult.No)
				{
					return;
				}
			}

			WinForms.Application.Exit();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref WinForms.Message m)
		{
			switch (m.Msg)
			{
				case 0x0010: // WM_CLOSE
					if (GlobalSettings.Instance().ExitOnX)
						MainForm.Exit();
					else
						this.Hide();
					return;

				case 0x0005: // WM_SIZE

					if ((int)m.WParam == 0x01) // SIZE_MINIMIZED
					{
						if (!GlobalSettings.Instance().DisplayTaskBar)
						{
							this.Hide();
							return;
						}
					}

					break;
			}

			base.WndProc(ref m);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnContactTreeDoubleClick(object sender, EventArgs e)
		{
			// determine what node we clicked on
			TreeNodeEx node = this.contactTreeView.GetNodeAt(
				this.contactTreeView.PointToClient(WinForms.Control.MousePosition)
			);

			if (node != null && node is FriendTreeNode)
			{
				// create a conversation box if we clicked on a friend's name.
				FriendTreeNode fnode = (FriendTreeNode)node;
				fnode.Protocol.StartConversation(fnode.Friend, false, null, new OperationCompleteEvent());
			}
		}
	}
}
