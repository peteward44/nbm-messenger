using System;
using Net = System.Net;
using Sockets = System.Net.Sockets;

// 9/6/03

namespace Proxy
{
	/// <summary>
	/// Summary description for HttpConnection.
	/// </summary>
	public class HttpConnection : IConnection
	{
		public HttpConnection()
		{
		}


		public bool Connected
		{
			get { return false; }
		}


		public void Connect(string proxyHostname, int proxyPort, string hostname, int port, ConnectionType type)
		{
		}


		public void Disconnect()
		{
		}


		public int Send(byte[] data)
		{
			return 0;
		}

		public int Send(byte[] data, int offset, int size)
		{
			return 0;
		}


		public IAsyncResult BeginSend(byte[] data, AsyncCallback callback, object state)	
		{
			return null;
		}

		public IAsyncResult BeginSend(byte[] data, int size, AsyncCallback callback, object state)
		{
			return null;
		}

		public IAsyncResult BeginSend(byte[] data, int offset, int size, AsyncCallback callback, object state)	
		{
			return null;
		}

		public int EndSend(IAsyncResult result)
		{
			return 0;
		}


		public int Receive(byte[] buffer)
		{
			return 0;
		}

		public int Receive(byte[] buffer, int size)
		{
			return 0;
		}

		public int Receive(byte[] buffer, int offset, int size)
		{
			return 0;
		}


		public IAsyncResult BeginReceive(byte[] buffer, AsyncCallback callback, object state)
		{
			return null;
		}

		public IAsyncResult BeginReceive(byte[] buffer, int size, AsyncCallback callback, object state)
		{
			return null;
		}

		public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			return null;
		}

		public int EndReceive(IAsyncResult result)
		{
			return 0;
		}


		public int SendTo(byte[] data)
		{
			return 0;
		}

		public int SendTo(byte[] data, int offset, int size)
		{
			return 0;
		}


		public IAsyncResult BeginSendTo(byte[] data, AsyncCallback callback, object state)
		{
			return null;
		}

		public IAsyncResult BeginSendTo(byte[] data, int size, AsyncCallback callback, object state)
		{
			return null;
		}

		public IAsyncResult BeginSendTo(byte[] data, int offset, int size, AsyncCallback callback, object state)
		{
			return null;
		}

		public int EndSendTo(IAsyncResult result)
		{
			return 0;
		}


		public int ReceiveFrom(byte[] buffer)
		{
			return 0;
		}

		public int ReceiveFrom(byte[] buffer, int size)
		{
			return 0;
		}

		public int ReceiveFrom(byte[] buffer, int offset, int size)
		{
			return 0;
		}


		public IAsyncResult BeginReceiveFrom(byte[] buffer, AsyncCallback callback, object state)
		{
			return null;
		}

		public IAsyncResult BeginReceiveFrom(byte[] buffer, int size, AsyncCallback callback, object state)
		{
			return null;
		}

		public IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			return null;
		}

		public int EndReceiveFrom(IAsyncResult result)
		{
			return 0;
		}
	}
}
