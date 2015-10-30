using System;

// 30/6/03

namespace NBM.Plugin
{
	/// <summary>
	/// Reason why the connection failed
	/// </summary>
	public enum ConnectionFailedReport
	{
		/// <summary>
		/// I dunno
		/// </summary>
		ConnectionFailed,
		/// <summary>
		/// Authentication failed
		/// </summary>
		AuthFailed,
		/// <summary>
		/// Protocol used by server is unrecognised by client
		/// </summary>
		InvalidProtocol
	}


	/// <summary>
	/// Special arguments for connection.
	/// Plugins must use this as an argument to OperationCompleteEvent.Execute()
	/// on IProtocol.Connect() 
	/// </summary>
	public class ConnectionCompleteArgs : OperationCompleteArgs
	{
		private ConnectionFailedReport report;


		/// <summary>
		/// Why the connection failed
		/// </summary>
		public ConnectionFailedReport Report
		{
			get { return this.report; }
		}


		/// <summary>
		/// Generates an appropriate error message
		/// </summary>
		/// <param name="report"></param>
		/// <returns></returns>
		private static string CreateErrorMessage(ConnectionFailedReport report)
		{
			switch (report)
			{
				default:
				case ConnectionFailedReport.ConnectionFailed:
					return "Connection to the remote host failed";
				case ConnectionFailedReport.AuthFailed:
					return "Authentication failed";
				case ConnectionFailedReport.InvalidProtocol:
					return "Unsupported protocol version - have you the latest NBM?";
			}
		}


		/// <summary>
		/// Constructs a ConnectionCompleteArgs reporting the connection was successful
		/// </summary>
		public ConnectionCompleteArgs()
			: base()
		{
		}


		/// <summary>
		/// Constructs a ConnectionCompleteArgs reporting the connection failed
		/// </summary>
		/// <param name="report"></param>
		public ConnectionCompleteArgs(ConnectionFailedReport report)
			: base(CreateErrorMessage(report), true)
		{
			this.report = report;
		}
		
	}
}
