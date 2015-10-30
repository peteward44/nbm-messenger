using System;
using System.Collections;
using NBM.Plugin;
using IO = System.IO;
using Net = System.Net;
using Sockets = System.Net.Sockets;

public delegate void ResponseReceivedHandler(MessageRouter router, Message message, object tag);


// 17/5/03

/// <summary>
/// Controls incoming and outgoing Yahoo protocol messages.
/// </summary>
public class MessageRouter
{
	private const int maxMessageSize = 2048;

	private ResponseReceivedHandler defaultMessageSink, onDisconnectHandler;

	private Proxy.IConnection connection;
	private Protocol protocol;

	private byte[] receiveBuffer = new byte[ maxMessageSize ];
	private CircularStream circStream = new CircularStream(maxMessageSize * 3);

	/// <summary>
	/// YahooService vs Pair(messagehandler, tag) for permanent events
	/// </summary>
	private Hashtable permanentEvents = new Hashtable();

	/// <summary>
	/// YahooService vs Pair(messagehandler, tag) for temporary events, ie they are removed
	/// from the hashtable once they are called. These are checked before permanent events.
	/// </summary>
	private Hashtable oneTimeEvents = new Hashtable(); 


	public volatile bool ExpectingDisconnection = false;



	public ResponseReceivedHandler DefaultMessageSink
	{
		get { return defaultMessageSink; }
		set { defaultMessageSink = value; }
	}


	private void onDisconnect(MessageRouter router, Message message, object tag)
	{
		if (ExpectingDisconnection)
			this.connection.Disconnect();
		else
		{
			if (this.onDisconnectHandler != null)
				this.onDisconnectHandler(router, message, tag);
		}
	}


	public MessageRouter(Protocol protocol, Proxy.IConnection connection, ResponseReceivedHandler defaultMessageSink, ResponseReceivedHandler onDisconnect)
	{
		this.protocol = protocol;
		this.connection = connection;
		this.defaultMessageSink = defaultMessageSink;
		this.onDisconnectHandler = onDisconnect;

		// set up receiving async call
		this.connection.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, new AsyncCallback(OnDataReceive), null);
	}



	public void Close()
	{
		if (this.connection != null)
			this.connection.Disconnect();
		this.connection = null;
	}


	public void AddPermanentEvent(YahooService service, ResponseReceivedHandler messageHandler, object tag)
	{
		if (!permanentEvents.Contains(service))
			permanentEvents.Add(service, new Pair(messageHandler, tag));
		else
			permanentEvents[service] = new Pair(messageHandler, tag);
	}


	public void AddPermanentEvent(YahooService service, ResponseReceivedHandler messageHandler)
	{
		AddPermanentEvent(service, messageHandler, null);
	}


	public void AddOneTimeEvent(YahooService service, ResponseReceivedHandler messageHandler, object tag)
	{
		if (!this.oneTimeEvents.Contains(service))
			this.oneTimeEvents.Add(service, new Pair(messageHandler, tag));
		else
			this.oneTimeEvents[service] = new Pair(messageHandler, tag);
	}


	public void AddOneTimeEvent(YahooService service, ResponseReceivedHandler messageHandler)
	{
		AddOneTimeEvent(service, messageHandler, null);
	}


	private void LogMessage(Message message, bool incoming)
	{
		this.protocol.control.WriteDebug("\r\n------------------------------------------------------");
		this.protocol.control.WriteDebug(((incoming) ? "Received:" : "Sent:"));
		this.protocol.control.WriteDebug("Service: " + message.Header.Service.ToString());
		this.protocol.control.WriteDebug("Status: " + message.Header.Status.ToString());
		this.protocol.control.WriteDebug("SessionID: 0x" + message.Header.SessionID.ToString("x"));

		foreach (Pair pair in message.Arguments)
		{
			int id = (int)pair.First;
			string content = System.Text.ASCIIEncoding.ASCII.GetString((byte[])pair.Second);

			this.protocol.control.WriteDebug("Arg " + id + " : " + content);
		}

		this.protocol.control.WriteDebug("------------------------------------------------------\r\n");
	}


	public void SendMessage(Message message)
	{
		try
		{
			byte[] raw = message.GetRawMessage();
			connection.Send(raw, 0, raw.Length);

			LogMessage(message, false);
		}
		catch
		{
			// couldn't write to the stream. we've been disconnected
			this.onDisconnect(this, null, null);
		}
	}


	private void OnDataReceive(IAsyncResult result)
	{
		if (connection == null)
			return;

		lock (this)
		{
			int bytesRead = connection.EndReceive(result);

			if (bytesRead > 0)
			{
				this.circStream.Write(this.receiveBuffer, 0, bytesRead);

				byte[] buffer = new byte[ this.circStream.DataAvailable ];
				this.circStream.Read(buffer, 0, buffer.Length);

				// if we have enough to read in the header
				if (buffer.Length >= Message.HeaderLength)
				{
					int bytesLeft = buffer.Length;
					int arrayPosition = 0;
					Message.MessageHeader header = new Message.MessageHeader();

					do
					{
						// read in the header
						if (bytesLeft >= Message.HeaderLength)
						{
							header.Set(buffer, arrayPosition);
							bytesLeft -= Message.HeaderLength;
						}
						else
						{
							AddToLeftOvers(buffer, arrayPosition, bytesLeft);
							break;
						}

						// if we have enough to read in the rest of the message,
						// copy it into a new array and dispatch the message
						if (bytesLeft >= header.Length)
						{
							byte[] message = new byte[ header.Length + Message.HeaderLength ];
							Array.Copy(buffer, arrayPosition, message, 0, message.Length);
							this.DispatchMessage(message);
							bytesLeft -= header.Length;
						}
							// otherwise, we dont have enough data to contruct a full message.
							// add the header and whatever else to the data "left overs" and go.
						else
						{
							AddToLeftOvers(buffer, arrayPosition, bytesLeft + Message.HeaderLength);
							break;
						}

						arrayPosition += Message.HeaderLength + header.Length;
					}
					while (bytesLeft > 0);
				}
				else
				{
					// else we dont have enough data to even construct the message header.
					// add it to the leftovers and forget about it.
					AddToLeftOvers(this.receiveBuffer, 0, bytesRead);
				}

				try
				{
					// restart async call
					this.connection.BeginReceive(receiveBuffer, 0, receiveBuffer.Length, new AsyncCallback(OnDataReceive), null);
				}
				catch
				{
					this.onDisconnect(this, null, null);
				}
			}
			else if (!ExpectingDisconnection)
			{
				this.onDisconnect(this, null, null);
			}
		}
	}


	private void AddToLeftOvers(byte[] rawMessage, int arrayIndex, int arraySize)
	{
		this.circStream.Write(rawMessage, arrayIndex, arraySize);
	}


	private void DispatchMessage(byte[] rawMessage)
	{
		Message message = new Message(rawMessage);

		LogMessage(message, true);

		try
		{
			// check if the code is registered temporarily
			if (this.oneTimeEvents.Contains(message.Header.Service))
			{
				try
				{
					Pair pair = (Pair)this.oneTimeEvents[message.Header.Service];
					((ResponseReceivedHandler)pair.First)(this, message, pair.Second);
				}
				catch
				{}

				this.oneTimeEvents.Remove(message.Header.Service);
			}
				// then check the permanent event table
			else if (this.permanentEvents.Contains(message.Header.Service))
			{
				Pair pair = (Pair)this.permanentEvents[message.Header.Service];
				((ResponseReceivedHandler)pair.First)(this, message, pair.Second);
			}
				// otherwise uses the default message sink
			else
			{
				if (defaultMessageSink != null)
					defaultMessageSink(this, message, null);
			}
		}
		catch
		{}
	}
}

