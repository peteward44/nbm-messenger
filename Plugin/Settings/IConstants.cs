using System;

// 1/6/03

namespace NBM.Plugin
{
	/// <summary>
	/// Holds protocol constants - all plugins must implement this interface
	/// </summary>
	public interface IConstants
	{
		/// <summary>
		/// True if the plugin supports file transfer during conversations
		/// </summary>
		bool SupportsFileTransfer
		{
			get;
		}


		/// <summary>
		/// True if the plugin supports sending a message when asking to be added to another user's
		/// contact list
		/// </summary>
		bool ReasonSentWithAddFriend
		{
			get;
		}


		/// <summary>
		/// Name of plugin
		/// </summary>
		string Name
		{
			get;
		}


		/// <summary>
		/// Default server host to connect to
		/// </summary>
		string DefaultServerHost
		{
			get;
		}


		/// <summary>
		/// Default server port to connect to
		/// </summary>
		int DefaultServerPort
		{
			get;
		}
	}
}
