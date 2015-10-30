using System;
using System.Collections;
using NBM.Plugin;

using IO = System.IO;
using Net = System.Net;
using Sockets = System.Net.Sockets;

using Regex = System.Text.RegularExpressions;


public delegate void ResponseReceivedHandler(MessageRouter router, Message message, object tag);


/// <summary>
/// Controls incoming and outgoing MSN protocol messages.
/// </summary>
public class MessageRouter
{
	private const int maxMessageSize = 2048;
	private const string md5MagicHash = @"Q1P7W2E4J9R8U3S5";

	private ResponseReceivedHandler defaultMessageSink, onDisconnectHandler;

	private Protocol protocol;
	private Proxy.IConnection connection;

	private byte[] receiveBuffer = new byte[ maxMessageSize ];
	private CircularStream circStream = new CircularStream(maxMessageSize * 3);

	private Hashtable codeEvents = new Hashtable(); // keeps a hashtable of strings (codes) for the key and events for the value.
	private Hashtable transactionEvents = new Hashtable(); // keeps a ht of ints (transaction IDs) for the key and events for the value.

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
		this.connection.BeginReceive(receiveBuffer, new AsyncCallback(OnDataReceive), null);
	}


	public void RemoveMessageEvent(Message message)
	{
		if (this.transactionEvents.Contains(message.TransactionID))
			this.transactionEvents.Remove(message.TransactionID);
	}


	public void AddCodeEvent(string code, ResponseReceivedHandler messageHandler)
	{
		AddCodeEvent(code, messageHandler, null);
	}


	public void AddCodeEvent(string code, ResponseReceivedHandler messageHandler, object tag)
	{
		if (!codeEvents.Contains(code))
			codeEvents.Add(code, new Pair(messageHandler, tag));
		else
			codeEvents[code] = new Pair(messageHandler, tag);
	}


	public void SendMessage(Message message)
	{
		SendMessage(message, null, null);
	}


	public void SendMessage(Message message, bool addLineEnding)
	{
		SendMessage(message, null, null, addLineEnding);
	}


	public void SendMessage(Message message, ResponseReceivedHandler responseReceivedEvent, object tag)
	{
		SendMessage(message, responseReceivedEvent, tag, true);
	}


	public void SendMessage(Message message, ResponseReceivedHandler responseReceivedEvent, object tag, bool addLineEnding)
	{
		byte[] byteMessage = System.Text.Encoding.ASCII.GetBytes(message.RawMessage + (addLineEnding ? "\r\n" : ""));

		try
		{
			this.connection.Send(byteMessage);

			this.protocol.control.WriteDebug("-----------------------------------------");
			this.protocol.control.WriteDebug("Sent");
			this.protocol.control.WriteDebug(System.Text.ASCIIEncoding.ASCII.GetString(byteMessage));
			this.protocol.control.WriteDebug("-----------------------------------------");

			if (responseReceivedEvent != null)
			{
				transactionEvents.Add(message.TransactionID, new Pair(responseReceivedEvent, tag));
			}
		}
		catch
		{
			// couldn't write to the stream. we've been disconnected
			this.onDisconnect(this, null, null);
		}
	}


	private void OnDataReceive(IAsyncResult result)
	{
		lock (this)
		{
			int bytesRead = this.connection.EndReceive(result);

			if (bytesRead > 0)
			{
				// shove data into memory stream
				circStream.Write(this.receiveBuffer, 0, bytesRead);

				// get raw data from circular stream
				byte[] rawData = new byte[ circStream.DataAvailable ];
				circStream.Read(rawData, 0, rawData.Length);

				// split the message into composite messages
				string rawMessage = System.Text.Encoding.ASCII.GetString(rawData);
				string subMessage = string.Empty;
				int position = 0;

				while (position < rawMessage.Length)
				{
					// see if we have enough to determine the message code
					if (rawMessage.Length - position >= 3)
					{
						string messageCode = rawMessage.Substring(position, 3);

						if (messageCode == "MSG")
						{
							// if its a MSG, then read until the length is satified
							Regex.Regex regex = new Regex.Regex(@"MSG\s+\S+\s+\S+\s+(?<length>\d+)");
							Regex.Match match = regex.Match(rawMessage.Substring(position));

							if (match.Success)
							{
								int length = int.Parse(match.Groups["length"].Value);
								int endOfHeader = rawMessage.IndexOf("\r\n", position);

								if (endOfHeader >= 0 && position + endOfHeader + length + 2 <= rawMessage.Length)
									subMessage = rawMessage.Substring(position, endOfHeader + length + 2 - position);
								else
								{
									this.SetToDataStream(rawMessage.Substring(position));
									break;
								}
							}
							else
							{
								// we didnt get enough of the string to determine the message length -
								// set it to the data stream and wait till the next rush of data
								this.SetToDataStream(rawMessage.Substring(position));
								break;
							}
						}
						else
						{
							// otherwise it isnt a MSG, so read up until the first \r\n
							int newPosition = rawMessage.IndexOf("\r\n", position);
							if (newPosition < 0)
							{
								// we dont have the entire message - clear out the left over data stream
								// and add the incomplete message to the stream
								this.SetToDataStream(rawMessage.Substring(position));
								break;
							}
							else
							{
								newPosition += 2;
								subMessage = rawMessage.Substring(position, newPosition - position);
							}
						}
					}
					else
					{
						// we dont even have enough to determine the message code - add it to the data stream
						// and forget about it
						this.SetToDataStream(rawMessage.Substring(position));
						break;
					}

					position += subMessage.Length;

					this.protocol.control.WriteDebug("-----------------------------------------");
					this.protocol.control.WriteDebug("Received");
					this.protocol.control.WriteDebug(subMessage);
					this.protocol.control.WriteDebug("-----------------------------------------");

					Message message = new Message(subMessage);

					switch (message.Code)
					{
						case "CHL":
						{
							// we received a challenge. Respond to it appropriately.
							string hash = message.Arguments;

							// md5 encrypt hash
							hash += md5MagicHash;

							MD5Encryption encryption = new MD5Encryption();
							string md5hash = encryption.Encrypt(hash);

							Message responseMessage = Message.ConstructMessage("QRY", @"msmsgs@msnmsgr.com 32"+"\r\n" + md5hash);
							this.SendMessage(responseMessage, false);
						}
							break;
					}

					try
					{

						// see if theres a transaction ID registered with it
						if (this.transactionEvents.Contains(message.TransactionID))
						{
							Pair pair = (Pair)this.transactionEvents[message.TransactionID];
							((ResponseReceivedHandler)pair.First)(this, message, pair.Second);
						}
							// else check if the code is registered
						else if (this.codeEvents.Contains(message.Code))
						{
							Pair pair = (Pair)this.codeEvents[message.Code];
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

				try
				{
					// restart async call
					this.connection.BeginReceive(receiveBuffer, new AsyncCallback(OnDataReceive), null);
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


	private void SetToDataStream(string message)
	{
		byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(message);
		this.circStream.Write(b, 0, message.Length);
	}


	public void Close()
	{
		this.connection.Disconnect();
	}
}

