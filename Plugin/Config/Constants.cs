using System;
using IO = System.IO;
using Win32 = Microsoft.Win32;

// 3/6/03

namespace NBM.Config
{
	/// <summary>
	/// Holds all constants throughout NBM.
	/// </summary>
	public class Constants
	{
		/// <summary>
		/// Private constructor as all members are static or const
		/// </summary>
		private Constants()
		{
		}


		/// <summary>
		/// Name of protocol class name to create from plugins
		/// </summary>
		public const string ProtocolClassName = "Protocol";

		/// <summary>
		/// Name of conversation class name to create from plugins
		/// </summary>
		public const string ConversationClassName = "Conversation";

		/// <summary>
		/// Name of settings class name to create from plugins
		/// </summary>
		public const string SettingsClassName = "Settings";

		/// <summary>
		/// Name of constants class name to create from plugins
		/// </summary>
		public const string ConstantsClassName = "Constants";


#if (DEBUG)
		private const string basePath = @"D:\programming\source\NBM\workingdirectory\";
#else
		private static readonly string basePath = System.Windows.Forms.Application.StartupPath + IO.Path.DirectorySeparatorChar;
#endif


		/// <summary>
		/// Path to images.
		/// </summary>
		public static string ImagePath
		{
			get { return basePath + "images"; }
		}


		/// <summary>
		/// Path to sounds.
		/// </summary>
		public static string SoundPath
		{
			get { return basePath + "sounds"; }
		}


#if (DEBUG)

		/// <summary>
		/// Path to plugins.
		/// </summary>
		public static string PluginsPath
		{
			get { return basePath + @"..\plugins\MSN\bin\debug"; }
		}

#else

		/// <summary>
		/// Path to plugins.
		/// </summary>
		public static string PluginsPath
		{
			get { return basePath + "plugins"; }
		}

#endif


		/// <summary>
		/// Path to user-specific data.
		/// </summary>
		public static string UserDataPath
		{
			get
			{
				char ds = IO.Path.DirectorySeparatorChar;

				string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
				int pos2 = username.LastIndexOf(@"\");
				if (pos2 > 0)
					username = username.Substring(pos2+1);

				return basePath + "userdata" + ds + username;
			}
		}


		/// <summary>
		/// Path to keep logs.
		/// </summary>
		public static string LogPath
		{
			get { return UserDataPath + IO.Path.DirectorySeparatorChar + "logs"; }
		}


		/// <summary>
		/// Registry key name pointing to where the startup stuff is kept
		/// </summary>
		public const string StartupRegistryPath = @"Software\Microsoft\Windows\CurrentVersion\Run";

		/// <summary>
		/// Startup registry root key
		/// </summary>
		public static readonly Win32.RegistryKey StartupRegistryRoot = Win32.Registry.CurrentUser;
	}
}
