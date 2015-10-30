using System;
using System.Collections;

// 1/6/03

namespace NBM.Plugin
{
	/// <summary>
	/// Base class for all protocol settings.
	/// Plugins dont have to derive from this class if they dont have
	/// any settings to save
	/// </summary>
	public class ProtocolSettings : ISettings
	{
		private IStorage storage;
		private IConstants constants;


		private bool rememberPassword = false, enabled = false;
		private string username = string.Empty, password = string.Empty, displayname = string.Empty, host;
		private int port = 0;
		private OnlineStatus status = OnlineStatus.Online;


		/// <summary>
		/// Constants object associated with settings
		/// </summary>
		public IConstants Constants
		{
			get { return constants; }
		}


		/// <summary>
		/// True if the protocol is enabled, false otherwise.
		/// </summary>
		public bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}


		/// <summary>
		/// User's username
		/// </summary>
		public string Username
		{
			get { return username; }
			set { username = value; }
		}


		/// <summary>
		/// User's password
		/// </summary>
		public string Password
		{
			get { return password; }
			set { password = value; }
		}


		/// <summary>
		/// True if the password is to be saved
		/// </summary>
		public bool RememberPassword
		{
			get { return rememberPassword; }
			set { rememberPassword = value; }
		}


		/// <summary>
		/// Displayname of user
		/// </summary>
		public string DisplayName
		{
			get { return (displayname != string.Empty) ? displayname : username; }
			set { displayname = value; }
		}


		/// <summary>
		/// Server to connect to
		/// </summary>
		public string ServerHost
		{
			get { return host; }
			set { host = value; }
		}


		/// <summary>
		/// Server port to connect to
		/// </summary>
		public int ServerPort
		{
			get { return port; }
			set { port = value; }
		}


		/// <summary>
		/// Status of user
		/// </summary>
		public OnlineStatus Status
		{
			get { return status; }
			set { status = value; }
		}


		/// <summary>
		/// Constructs a ProtocolSettings
		/// </summary>
		/// <param name="constants"></param>
		/// <param name="storage"></param>
		/// <param name="optionsList"></param>
		public ProtocolSettings(IConstants constants, IStorage storage, ArrayList optionsList)
		{
			this.storage = storage;
			this.constants = constants;
		}


		/// <summary>
		/// 
		/// </summary>
		public virtual void Load()
		{
			this.enabled = this.storage.ReadBool("Enabled", false);
			this.username = this.storage.ReadString("Username", string.Empty);
			this.password = this.storage.ReadString("Password", string.Empty);
			this.host = this.storage.ReadString("ServerHost", this.constants.DefaultServerHost);
			this.port = this.storage.ReadInt32("ServerPort", this.constants.DefaultServerPort);
			this.status = (OnlineStatus)this.storage.ReadInt32("Status", (int)OnlineStatus.Online);
			this.rememberPassword = this.storage.ReadBool("RememberPassword", false);
		}


		/// <summary>
		/// 
		/// </summary>
		public virtual void Save()
		{
			this.storage.Write("Enabled", this.enabled);
			this.storage.Write("Username", this.username);
			this.storage.Write("Password", ((this.rememberPassword) ? this.password : string.Empty));
			this.storage.Write("RememberPassword", this.rememberPassword);
			this.storage.Write("ServerHost", this.ServerHost);
			this.storage.Write("ServerPort", this.ServerPort);
			this.storage.Write("Status", (int)this.status);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return constants.Name.GetHashCode();
		}
	}
}
