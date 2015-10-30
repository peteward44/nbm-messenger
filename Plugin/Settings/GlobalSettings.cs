using System;
using Win32 = Microsoft.Win32;

// 17/6/03

namespace NBM.Plugin.Settings
{
	#region Public enums

	/// <summary>
	/// Type of internet connection to use
	/// </summary>
	public enum InternetConnection
	{
		/// <summary>
		/// Direct connection - no proxies or anything
		/// </summary>
		Direct,
		/// <summary>
		/// SOCKSv4 connection
		/// </summary>
		Socks4,
		/// <summary>
		/// SOCKSv5 connection
		/// </summary>
		Socks5,
		/// <summary>
		/// HTTP proxy server connection
		/// </summary>
		Http
	}


	/// <summary>
	/// Type of log to create
	/// </summary>
	public enum LogType
	{
		/// <summary>
		/// Text log
		/// </summary>
		Text,
		/// <summary>
		/// HTML log
		/// </summary>
		Html
	}


	/// <summary>
	/// What happens when a friend comes online
	/// </summary>
	public enum FriendOnlineEvent
	{
		/// <summary>
		/// Does nothing
		/// </summary>
		DoNothing,
		/// <summary>
		/// Balloon tool tip appears above the system tray icon
		/// </summary>
		BalloonToolTip,
		/// <summary>
		/// MSN-style popup above system tray icon
		/// </summary>
		MSNPopUp
	}


	/// <summary>
	/// What happens when a friend messages the user
	/// </summary>
	public enum FriendMessageEvent
	{
		/// <summary>
		/// Does nothing
		/// </summary>
		DoNothing,
		/// <summary>
		/// Flashes the message window
		/// </summary>
		FlashWindow
	}


	#endregion


	/// <summary>
	/// Global settings. Its a singleton so we can implement from ISettings.
	/// </summary>
	public class GlobalSettings : ISettings
	{
		private IStorage storage;


		#region Public members


		/// <summary>
		/// Type of log to use
		/// </summary>
		public LogType LogType = LogType.Text;

		/// <summary>
		/// 
		/// </summary>
		public bool LoadOnStartup;

		/// <summary>
		/// 
		/// </summary>
		public bool AlwaysOnTop;

		/// <summary>
		/// 
		/// </summary>
		public bool HideOfflineContacts;

		/// <summary>
		/// 
		/// </summary>
		public bool ConnectAllOnStartup;

		/// <summary>
		/// 
		/// </summary>
		public bool ExitOnX;

		/// <summary>
		/// 
		/// </summary>
		public bool DisplayTaskBar;

		/// <summary>
		/// 
		/// </summary>
		public bool PromptOnExit;

		/// <summary>
		/// 
		/// </summary>
		public bool UseProxyAuthentication;

		/// <summary>
		/// 
		/// </summary>
		public bool TimeStampConversations;

		/// <summary>
		/// 
		/// </summary>
		public bool ReconnectOnDisconnection;

		/// <summary>
		/// 
		/// </summary>
		public bool SendTypingNotifications;

		/// <summary>
		/// 
		/// </summary>
		public bool ShowEmoticons;

		/// <summary>
		/// 
		/// </summary>
		public InternetConnection InternetConnection;

		/// <summary>
		/// 
		/// </summary>
		public string ProxyUsername;

		/// <summary>
		/// 
		/// </summary>
		public string ProxyPassword;

		/// <summary>
		/// 
		/// </summary>
		public string ProxyServerHost;

		/// <summary>
		/// 
		/// </summary>
		public int ProxyServerPort;

		/// <summary>
		/// 
		/// </summary>
		public bool RememberProxyPassword;

		/// <summary>
		/// 
		/// </summary>
		public string TimeStampFormat;

		/// <summary>
		/// 
		/// </summary>
		public System.Drawing.Point ContactListLocation = System.Drawing.Point.Empty;

		/// <summary>
		/// 
		/// </summary>
		public System.Drawing.Size ContactListSize = System.Drawing.Size.Empty;

		/// <summary>
		/// 
		/// </summary>
		public FriendOnlineEvent FriendOnlineEvent;
		
		/// <summary>
		/// 
		/// </summary>
		public FriendMessageEvent FriendMessageEvent;

		/// <summary>
		/// 
		/// </summary>
		public int NumTimesToFlashOnMessageReceived;

		/// <summary>
		/// 
		/// </summary>
		public bool PlaySoundOnFriendOnline;

		/// <summary>
		/// 
		/// </summary>
		public bool PlaySoundOnMessageReceived;

		/// <summary>
		/// 
		/// </summary>
		public string SoundOnFriendOnline;

		/// <summary>
		/// 
		/// </summary>
		public string SoundOnMessageReceived;


		#endregion


		/// <summary>
		/// 
		/// </summary>
		/// <param name="storage"></param>
		private GlobalSettings(IStorage storage)
		{
			this.storage = storage;

			this.InternetConnection = InternetConnection.Direct;
			LoadOnStartup = AlwaysOnTop = HideOfflineContacts = DisplayTaskBar = ConnectAllOnStartup = false;
			PromptOnExit = TimeStampConversations = ReconnectOnDisconnection = SendTypingNotifications = true;
			UseProxyAuthentication = ExitOnX = RememberProxyPassword = false;
			ProxyUsername = ProxyPassword = ProxyServerHost = string.Empty;
		}


		/// <summary>
		/// 
		/// </summary>
		public void Save()
		{
			storage.Write("SoundOnFriendOnline", SoundOnFriendOnline);
			storage.Write("SoundOnMessageReceived", SoundOnMessageReceived);

			storage.Write("AlwaysOnTop", this.AlwaysOnTop);
			storage.Write("ConnectAllOnStartup", this.ConnectAllOnStartup);
			storage.Write("HideOfflineContacts", this.HideOfflineContacts);
			storage.Write("DisplayTaskBar", this.DisplayTaskBar);
			storage.Write("PromptOnExit", this.PromptOnExit);
			storage.Write("InternetConnection", (int)this.InternetConnection);
			storage.Write("ReconnectOnDisconnection", this.ReconnectOnDisconnection);
			storage.Write("ExitOnX", this.ExitOnX);

			storage.Write("ProxyUsername", this.ProxyUsername);
			storage.Write("ProxyPassword", (this.RememberProxyPassword) ? this.ProxyPassword : string.Empty);
			storage.Write("RememberProxyPassword", this.RememberProxyPassword);
			storage.Write("UseProxyAuthentication", this.UseProxyAuthentication);
			storage.Write("ProxyServerHost", this.ProxyServerHost);
			storage.Write("ProxyServerPort", this.ProxyServerPort);

			storage.Write("ContactListWidth", this.ContactListSize.Width);
			storage.Write("ContactListHeight", this.ContactListSize.Height);

			storage.Write("TimeStampConversations", this.TimeStampConversations);
			storage.Write("TimeStampFormat", this.TimeStampFormat);
			storage.Write("ShowEmoticons", this.ShowEmoticons);
			storage.Write("SendTypingNotifications", this.SendTypingNotifications);

			System.Drawing.Rectangle screenSize = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;

			if (this.ContactListLocation.X > screenSize.Width)
				this.ContactListLocation.X = screenSize.Width - this.ContactListSize.Width;
			if (this.ContactListLocation.X < 0)
				this.ContactListLocation.X = 1;

			if (this.ContactListLocation.Y > screenSize.Height)
				this.ContactListLocation.Y = screenSize.Height - this.ContactListSize.Height;
			if (this.ContactListLocation.Y < 0)
				this.ContactListLocation.Y = 1;

			storage.Write("ContactListX", this.ContactListLocation.X);
			storage.Write("ContactListY", this.ContactListLocation.Y);


			Win32.RegistryKey startupKey =
				Config.Constants.StartupRegistryRoot.OpenSubKey(Config.Constants.StartupRegistryPath, true);

			if (this.LoadOnStartup)
				startupKey.SetValue("NBM", System.Windows.Forms.Application.ExecutablePath);
			else
				startupKey.DeleteValue("NBM", false);

			startupKey.Close();


			storage.Write("FriendOnlineEvent", (int)this.FriendOnlineEvent);
			storage.Write("FriendMessageEvent", (int)this.FriendMessageEvent);
			storage.Write("NumTimesToFlashOnMessageReceived", this.NumTimesToFlashOnMessageReceived);

			storage.Write("PlaySoundOnFriendOnline", this.PlaySoundOnFriendOnline);
			storage.Write("PlaySoundOnMessageReceived", this.PlaySoundOnMessageReceived);

			storage.Write("LogType", (int)this.LogType);
		}


		/// <summary>
		/// 
		/// </summary>
		public void Load()
		{
			SoundOnFriendOnline = storage.ReadString("SoundOnFriendOnline",
				Config.Constants.SoundPath + System.IO.Path.DirectorySeparatorChar + "friendonline.wav");
			SoundOnMessageReceived = storage.ReadString("SoundOnMessageReceived",
				Config.Constants.SoundPath + System.IO.Path.DirectorySeparatorChar + "messagereceived.wav");

			this.AlwaysOnTop = storage.ReadBool("AlwaysOnTop", false);
			this.ConnectAllOnStartup = storage.ReadBool("ConnectAllOnStartup", false);
			this.HideOfflineContacts = storage.ReadBool("HideOfflineContacts", false);
			this.DisplayTaskBar = storage.ReadBool("DisplayTaskBar", true);
			this.PromptOnExit = storage.ReadBool("PromptOnExit", true);
			this.InternetConnection = (InternetConnection)storage.ReadInt32("InternetConnection", (int)InternetConnection.Direct);
			this.ExitOnX = storage.ReadBool("ExitOnX", false);

			this.ProxyUsername = storage.ReadString("ProxyUsername", "");
			this.ProxyPassword = storage.ReadString("ProxyPassword", "");
			this.RememberProxyPassword = storage.ReadBool("RememberProxyPassword", false);
			this.ProxyServerHost = storage.ReadString("ProxyServerHost", "");
			this.ProxyServerPort = storage.ReadInt32("ProxyServerPort", 0);
			this.UseProxyAuthentication = storage.ReadBool("UseProxyAuthentication", false);

			this.TimeStampConversations = storage.ReadBool("TimeStampConversations", true);
			this.SendTypingNotifications = storage.ReadBool("SendTypingNotifications", true);
			this.TimeStampFormat = storage.ReadString("TimeStampFormat", "[%h:%m]");
			this.ShowEmoticons = storage.ReadBool("ShowEmoticons", true);

			this.ContactListSize.Width = storage.ReadInt32("ContactListWidth", 100);
			this.ContactListSize.Height = storage.ReadInt32("ContactListHeight", 300);
			this.ContactListLocation.X = storage.ReadInt32("ContactListX", 200);
			this.ContactListLocation.Y = storage.ReadInt32("ContactListY", 200);


			this.FriendOnlineEvent = (FriendOnlineEvent)storage.ReadInt32("FriendOnlineEvent", (int)FriendOnlineEvent.BalloonToolTip);
			this.FriendMessageEvent = (FriendMessageEvent)storage.ReadInt32("FriendMessageEvent", (int)FriendMessageEvent.FlashWindow);
			this.NumTimesToFlashOnMessageReceived = storage.ReadInt32("NumTimesToFlashOnMessageReceived", 5);

			this.PlaySoundOnFriendOnline = storage.ReadBool("PlaySoundOnFriendOnline", true);
			this.PlaySoundOnMessageReceived = storage.ReadBool("PlaySoundOnMessageReceived", true);

			this.LogType = (LogType)storage.ReadInt32("LogType", (int)LogType.Text);


			Win32.RegistryKey startupKey = Config.Constants.StartupRegistryRoot.OpenSubKey(Config.Constants.StartupRegistryPath, true);
			this.LoadOnStartup = startupKey.GetValue("NBM") != null;
			startupKey.Close();
		}


		#region Singleton methods


		private static GlobalSettings instance = null;


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static GlobalSettings Instance()
		{
			if (instance == null)
				throw new ApplicationException("GlobalSettings has not been set a storage type");
			return instance;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="storage"></param>
		/// <returns></returns>
		public static GlobalSettings Instance(IStorage storage)
		{
			if (storage == null)
				throw new ArgumentNullException("Storage cannot be null");

			if (instance == null)
				instance = new GlobalSettings(storage);
			return instance;
		}


		#endregion

	}
}
