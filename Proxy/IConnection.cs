using System;
using Net = System.Net;
using Sockets = System.Net.Sockets;

// 9/6/03

namespace Proxy
{
	public enum ConnectionType
	{
		Tcp, Udp
	}


	/// <summary>
	/// Base interface for connections
	/// </summary>
	public interface IConnection
	{
		bool Connected
		{
			get;
		}


		void Connect(string proxyHostname, int proxyPort, string hostname, int port, ConnectionType type);
		void Disconnect();


		int Send(byte[] data);
		int Send(byte[] data, int offset, int size);

		IAsyncResult BeginSend(byte[] data, AsyncCallback callback, object state);
		IAsyncResult BeginSend(byte[] data, int size, AsyncCallback callback, object state);
		IAsyncResult BeginSend(byte[] data, int offset, int size, AsyncCallback callback, object state);

		int EndSend(IAsyncResult result);


		int Receive(byte[] buffer);
		int Receive(byte[] buffer, int size);
		int Receive(byte[] buffer, int offset, int size);

		IAsyncResult BeginReceive(byte[] buffer, AsyncCallback callback, object state);
		IAsyncResult BeginReceive(byte[] buffer, int size, AsyncCallback callback, object state);
		IAsyncResult BeginReceive(byte[] buffer, int offset, int size, AsyncCallback callback, object state);

		int EndReceive(IAsyncResult result);


		int SendTo(byte[] data);
		int SendTo(byte[] data, int offset, int size);

		IAsyncResult BeginSendTo(byte[] data, AsyncCallback callback, object state);
		IAsyncResult BeginSendTo(byte[] data, int size, AsyncCallback callback, object state);
		IAsyncResult BeginSendTo(byte[] data, int offset, int size, AsyncCallback callback, object state);

		int EndSendTo(IAsyncResult result);


		int ReceiveFrom(byte[] buffer);
		int ReceiveFrom(byte[] buffer, int size);
		int ReceiveFrom(byte[] buffer, int offset, int size);

		IAsyncResult BeginReceiveFrom(byte[] buffer, AsyncCallback callback, object state);
		IAsyncResult BeginReceiveFrom(byte[] buffer, int size, AsyncCallback callback, object state);
		IAsyncResult BeginReceiveFrom(byte[] buffer, int offset, int size, AsyncCallback callback, object state);

		int EndReceiveFrom(IAsyncResult result);
	}
}
