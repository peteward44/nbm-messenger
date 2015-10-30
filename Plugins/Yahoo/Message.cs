using System;
using System.Collections;
using NBM.Plugin;
using System.Text;

using IO = System.IO;

// 15/5/03


public class Message
{
	private ArrayList argumentList = new ArrayList();

	public static readonly byte[] ArgumentSeperator = new byte[] { 0xC0, 0x80 };
	public const int HeaderLength = 20;
	public const int Version = 10;


	/// <summary>
	/// Message header.
	/// </summary>
	public class MessageHeader
	{
		private UInt32 packetIdentifier = 0x47534D59; // packet identifier - "YMSG"
		private UInt32 version = Message.Version; // Version (little endian)
		private UInt16 length = 0; // Length of the packet (big endian)
		private UInt16 service = 0; // Service type (big endian)
		private UInt32 status = 0; // Status of user (online, invis, etc) (big endian)
		private UInt32 sessionIdentifier = 0; // Unique number given to each session.


		public YahooService Service
		{
			get { return (YahooService)Endian.Convert(service); }
			set { service = Endian.Convert((UInt16)value); }
		}


		public YahooStatus Status
		{
			get { return (YahooStatus)Endian.Convert(status); }
			set { status = Endian.Convert((UInt32)value); }
		}


		public UInt32 SessionID
		{
			get { return sessionIdentifier; }
		}


		public UInt16 Length
		{
			get { return Endian.Convert(length); }
			set { length = Endian.Convert(value); }
		}


		public UInt32 SessionIdentifier
		{
			get { return sessionIdentifier; }
			set { sessionIdentifier = value; }
		}


		public void Set(byte[] b, int arrayIndex)
		{
			System.IO.MemoryStream memStream = new System.IO.MemoryStream(b);
			memStream.Seek(arrayIndex, IO.SeekOrigin.Begin);
			System.IO.BinaryReader br = new System.IO.BinaryReader(memStream);

			packetIdentifier = br.ReadUInt32();
			version = br.ReadUInt32();
			length = br.ReadUInt16();
			service = br.ReadUInt16();
			status = br.ReadUInt32();
			sessionIdentifier = br.ReadUInt32();

			br.Close();
			memStream.Close();
		}


		public void Get(IO.BinaryWriter bw)
		{
			bw.Write(packetIdentifier);
			bw.Write(version);
			bw.Write(length);
			bw.Write(service);
			bw.Write(status);
			bw.Write(sessionIdentifier);
		}
	}


	private MessageHeader header = new MessageHeader();


	public MessageHeader Header
	{
		get { return header; }
	}


	#region Constructors


	public Message(UInt32 sessionID)
		: this(sessionID, YahooService.None)
	{
	}


	public Message(UInt32 sessionID, YahooService service)
		: this(sessionID, service, YahooStatus.Available)
	{
	}


	public Message(UInt32 sessionID, YahooService service, YahooStatus status)
	{
		header.SessionIdentifier = sessionID;
		header.Service = service;
		header.Status = status;
	}


	/// <summary>
	/// This constructor is for incoming messages
	/// </summary>
	/// <param name="rawMessage"></param>
	public Message(byte[] rawMessage)
	{
		// copy header
		header.Set(rawMessage, 0);

		// divide into arguments
		bool isKey = true;
		byte[] content = new byte[ header.Length ]; // content cant be longer than the message
		int contentLength = 0;
		int key = 0;

		for (int i=Message.HeaderLength; i<header.Length+Message.HeaderLength; ++i)
		{
			if (rawMessage[i] == Message.ArgumentSeperator[0]
				&& rawMessage[i+1] == Message.ArgumentSeperator[1])
			{
				if (contentLength > 0)
				{
					// we have hit a boundary
					if (isKey)
					{
						key = int.Parse( ASCIIEncoding.ASCII.GetString( content, 0, contentLength ) );
					}
					else
					{
						byte[] temp = new byte[contentLength];
						Array.Copy(content, 0, temp, 0, contentLength);
						this.argumentList.Add( new Pair(key, temp) );
					}
				}

				contentLength = 0;
				++i;
				isKey = !isKey;
			}
			else
			{
				content[contentLength++] = rawMessage[i];
			}
		}
	}


	#endregion


	public void AddArgument(int id, byte[] contents)
	{
		this.argumentList.Add(new Pair(id, contents));

		// calculate length of arguments
		byte[] idbyte = ASCIIEncoding.ASCII.GetBytes(string.Format("{0}", id));
		this.Header.Length += (UInt16)(idbyte.Length + contents.Length + (Message.ArgumentSeperator.Length * 2));
	}


	public void AddArgument(int id, string contents)
	{
		AddArgument(id, ASCIIEncoding.ASCII.GetBytes(contents));
	}


	public ArrayList Arguments
	{
		get { return this.argumentList; }
	}


	public byte[] GetArgument(int id)
	{
		foreach (Pair pair in this.argumentList)
		{
			if ((int)pair.First == id)
			{
				return (byte[])pair.Second;
			}
		}

		return null;
	}


	public string GetArgumentString(int id)
	{
		byte[] arg = GetArgument(id);
		return (arg != null) ? ASCIIEncoding.ASCII.GetString(arg) : null;
	}



	public byte[] GetRawMessage()
	{
		IO.MemoryStream stream = new IO.MemoryStream(Message.HeaderLength);
		IO.BinaryWriter bw = new IO.BinaryWriter(stream);

		// write header
		header.Get(bw);

		// write arguments
		foreach (Pair pair in this.argumentList)
		{
			int key = (int)pair.First;
			byte[] val = (byte[])pair.Second;

			bw.Write( ASCIIEncoding.ASCII.GetBytes(string.Format("{0}", key)) );
			bw.Write( Message.ArgumentSeperator );
			bw.Write( val );
			bw.Write( Message.ArgumentSeperator );
		}

		byte[] final = stream.ToArray();

		bw.Close();
		stream.Close();

		return final;
	}
}


