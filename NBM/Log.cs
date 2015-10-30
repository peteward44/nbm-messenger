using System;
using IO = System.IO;
using NBM.Plugin;

// 13/6/03

namespace NBM
{
	/// <summary>
	/// Provides base interface for message and event logging
	/// </summary>
	public interface ILog : IConversationListener, IDisposable
	{
	}


	/// <summary>
	/// Generates appropriate ILog implementor depending on user settings.
	/// </summary>
	public class LogFactory
	{
		private LogFactory()
		{}


		/// <summary>
		/// Generates appropriate ILog implementor depending on user settings.
		/// </summary>
		/// <param name="protocol">Appropriate protocol</param>
		/// <param name="friendName">Original friend within the conversation</param>
		/// <returns></returns>
		public static ILog CreateLog(Protocol protocol, string friendName)
		{
			switch (NBM.Plugin.Settings.GlobalSettings.Instance().LogType)
			{
				default:
				case NBM.Plugin.Settings.LogType.Text:
					return new PlainTextLog(protocol, friendName);

				case NBM.Plugin.Settings.LogType.Html:
					return new HtmlLog(protocol, friendName);
			}
		}
	}


	/// <summary>
	/// Plain text conversation logging.
	/// </summary>
	public class PlainTextLog : ILog
	{
		private IO.StreamWriter streamWriter;
		private Protocol protocol;


		/// <summary>
		/// Constructs a PlainTextLog
		/// </summary>
		/// <param name="protocol">Protocol to log</param>
		/// <param name="friendName">Friend's name of conversation</param>
		public PlainTextLog(Protocol protocol, string friendName)
		{
			this.protocol = protocol;

			IO.Directory.CreateDirectory(Config.Constants.LogPath + IO.Path.DirectorySeparatorChar + protocol.Name);

			string path = Config.Constants.LogPath + IO.Path.DirectorySeparatorChar
				+ protocol.Name + IO.Path.DirectorySeparatorChar + friendName + ".txt";
			this.streamWriter = new IO.StreamWriter(path, true);
		}


		/// <summary>
		/// Disposes the object
		/// </summary>
		public void Dispose()
		{
			if (this.streamWriter != null)
			{
				this.streamWriter.Close();
				this.streamWriter = null;
			}
		}


		#region ConversationListener methods


		/// <summary>
		/// 
		/// </summary>
		public void OnConversationConnected()
		{
			DateTime now = DateTime.Now;
			this.streamWriter.WriteLine();
			this.streamWriter.WriteLine("Conversation started at "
				+ Time.GetFormattedTime()
				+ " on "
				+ now.Day + "/" + now.Month + "/" + now.Year);
			this.streamWriter.WriteLine();
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnConversationDisconnected()
		{
			DateTime now = DateTime.Now;
			this.streamWriter.WriteLine();
			this.streamWriter.WriteLine("Conversation finished at "
				+ Time.GetFormattedTime()
				+ " on "
				+ now.Day + "/" + now.Month + "/" + now.Year);
			this.streamWriter.WriteLine();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		public void OnUserSay(string text)
		{
			this.streamWriter.WriteLine("[" + Time.GetFormattedTime() + "] " + protocol.Settings.DisplayName + ": " + text);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="text"></param>
		public void OnFriendSay(Friend friend, string text)
		{
			this.streamWriter.WriteLine("[" + Time.GetFormattedTime() + "] " + friend.DisplayName + ": " + text);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnFriendJoin(Friend friend)
		{
//			this.streamWriter.WriteLine("[" + Time.GetFormattedTime() + "] " + friend.DisplayName + ": has joined the conversation");
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnFriendLeave(Friend friend)
		{
//			this.streamWriter.WriteLine("[" + Time.GetFormattedTime() + "] " + friend.DisplayName + ": has left the conversation");
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnTypingNotification(Friend friend)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="filename"></param>
		public void OnFileSendInvitation(Friend friend, string filename)
		{
		}

		#endregion
	}


	/// <summary>
	/// HTML logging
	/// </summary>
	public class HtmlLog : ILog
	{
		/// <summary>
		/// Constructs HtmlLog
		/// </summary>
		/// <param name="protocol">Protocol to log</param>
		/// <param name="friendName">Friend's name</param>
		public HtmlLog(Protocol protocol, string friendName)
		{
		}


		/// <summary>
		/// Disposes the object
		/// </summary>
		public void Dispose()
		{
		}


		#region ConversationListener methods


		/// <summary>
		/// 
		/// </summary>
		public void OnConversationConnected()
		{
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnConversationDisconnected()
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="text"></param>
		public void OnFriendSay(Friend friend, string text)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnFriendJoin(Friend friend)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnFriendLeave(Friend friend)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void OnTypingNotification(Friend friend)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		public void OnUserSay(string text)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="filename"></param>
		public void OnFileSendInvitation(Friend friend, string filename)
		{
		}

		#endregion
	}
}
