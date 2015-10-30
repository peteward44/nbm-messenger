using System;
using Net = System.Net;
using Sockets = System.Net.Sockets;

// 9/6/03

namespace Proxy
{
	/// <summary>
	/// Simply a shell to the normal sockets
	/// </summary>
	public class DirectConnection : IConnection
	{
		private Sockets.Socket socket;
		private Net.IPEndPoint endpoint;


		public bool Connected
		{
			get { return socket != null && socket.Connected; }
		}


		public DirectConnection()
		{
		}


		public void Connect(string proxyHostname, int proxyPort, string hostname, int port, ConnectionType type)
		{
			if (Connected)
				Disconnect();

			this.socket = new Sockets.Socket(
				Sockets.AddressFamily.InterNetwork,
				(type == ConnectionType.Tcp) ? Sockets.SocketType.Stream : Sockets.SocketType.Dgram,
				(type == ConnectionType.Tcp) ? Sockets.ProtocolType.Tcp : Sockets.ProtocolType.Udp);

			this.endpoint = new Net.IPEndPoint( Net.Dns.GetHostByName(hostname).AddressList[0], port );

//			if (type == ConnectionType.Tcp)
				this.socket.Connect( endpoint );
		}


		public void Disconnect()
		{
			if (socket != null)
				this.socket.Close();
			this.socket = null;
		}


		public int Send(byte[] data)
		{
			return this.socket.Send(data);
		}

		public int Send(byte[] data, int offset, int size)
		{
			return this.socket.Send(data, offset, size, Sockets.SocketFlags.None);
		}


		public IAsyncResult BeginSend(byte[] data, AsyncCallback callback, object state)	
		{
			return this.socket.BeginSend(data, 0, data.Length, Sockets.SocketFlags.None, callback, state);
		}

		public IAsyncResult BeginSend(byte[] data, int size, AsyncCallback callback, object state)
		{
			return this.socket.BeginSend(data, 0, size, Sockets.SocketFlags.None, callback, state);
		}

		public IAsyncResult BeginSend(byte[] data, int offset, int size, AsyncCallback callback, object state)	
		{
			return this.socket.BeginSend(data, offset, size, Sockets.SocketFlags.None, callback, state);
		}

		public int EndSend(IAsyncResult result)
		{
			return this.socket.EndSend(result);
		}


		public int Receive(byte[] buffer)
		{
			return this.socket.Receive(buffer);
		}

		public int Receive(byte[] buffer, int size)
		{
			return this.socket.Receive(buffer, size, Sockets.SocketFlags.None);
		}

		public int Receive(byte[] buffer, int offset, int size)
		{
			return this.socket.Receive(buffer, offset, size, Sockets.SocketFlags.None);
		}


		public IAsyncResult BeginReceive(byte[] buffer, AsyncCallback callback, object state)
		{
			return this.socket.BeginReceive(buffer, 0, buffer.Length, Sockets.SocketFlags.None, callback, state);
		}

		public IAsyncResult BeginReceive(byte[] buffer, int size, AsyncCallback callback, object state)
		{
			return this.socket.BeginReceive(buffer, 0, size, Sockets.SocketFlags.None, callback, state);
		}

		public IAsyncResult BeginReceive(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			return this.socket.BeginReceive(buffer, offset, buffer.Length, Sockets.SocketFlags.None, callback, state);
		}

		public int EndReceive(IAsyncResult result)
		{
			if (this.socket == null || !this.socket.Connected)
				return -1;
			else
			{
				try
				{
					return this.socket.EndReceive(result);
				}
				catch
				{
					return -1;
				}
			}
		}


		public int SendTo(byte[] data)
		{
			return this.socket.SendTo(data, this.endpoint);
		}

		public int SendTo(byte[] data, int offset, int size)
		{
			return this.socket.SendTo(data, offset, size, Sockets.SocketFlags.None, this.endpoint);
		}


		public IAsyncResult BeginSendTo(byte[] data, AsyncCallback callback, object state)
		{
			return this.socket.BeginSendTo(data, 0, data.Length, Sockets.SocketFlags.None, this.endpoint, callback, state);
		}

		public IAsyncResult BeginSendTo(byte[] data, int size, AsyncCallback callback, object state)
		{
			return this.socket.BeginSendTo(data, 0, size, Sockets.SocketFlags.None, this.endpoint, callback, state);
		}

		public IAsyncResult BeginSendTo(byte[] data, int offset, int size, AsyncCallback callback, object state)
		{
			return this.socket.BeginSendTo(data, offset, size, Sockets.SocketFlags.None, this.endpoint, callback, state);
		}


		public int EndSendTo(IAsyncResult result)
		{
			return this.socket.EndSendTo(result);
		}


		public int ReceiveFrom(byte[] buffer)
		{
			return this.ReceiveFrom(buffer);
		}

		public int ReceiveFrom(byte[] buffer, int size)
		{
			return this.ReceiveFrom(buffer, size);
		}

		public int ReceiveFrom(byte[] buffer, int offset, int size)
		{
			return this.ReceiveFrom(buffer, offset, size);
		}


		public IAsyncResult BeginReceiveFrom(byte[] buffer, AsyncCallback callback, object state)
		{
			return this.BeginReceiveFrom(buffer, callback, state);
		}

		public IAsyncResult BeginReceiveFrom(byte[] buffer, int size, AsyncCallback callback, object state)
		{
			return this.BeginReceiveFrom(buffer, size, callback, state);
		}

		public IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			return this.BeginReceiveFrom(buffer, offset, size, callback, state);
		}

		public int EndReceiveFrom(IAsyncResult result)
		{
			return this.EndReceiveFrom(result);
		}
	}
}
