using System;
using System.Collections;

using Xml = System.Xml;
using IO = System.IO;

// 16/6/03

/// <summary>
/// Stores key-value configuration pairs in an xml file.
/// </summary>
public class XmlStorage : IStorage
{
	private const string rootName = "storage";
	private const string keyName = "key";
	private const string subsectionName = "subsection";

	private bool isRoot = false;
	private string fileName = string.Empty;
	private Hashtable keyTable = new Hashtable();
	private Hashtable subsectionTable = new Hashtable();

	private XmlStorage root = null;
	private string name;


	/// <summary>
	/// Constructs an XmlStorage.
	/// This is used to create sub-sections
	/// </summary>
	/// <param name="root">Root XmlStorage instance</param>
	/// <param name="name">Name of subsection</param>
	private XmlStorage(XmlStorage root, string name)
	{
		this.isRoot = false;
		this.root = root;
		this.name = name;
	}


	/// <summary>
	/// Constructs an XmlStorage.
	/// This is used to create sub-sections
	/// </summary>
	/// <param name="root">Root XmlStorage instance</param>
	/// <param name="name">Name of subsection</param>
	/// <param name="reader">Xml reader to read elements from</param>
	private XmlStorage(XmlStorage root, string name, Xml.XmlTextReader reader)
	{
		this.isRoot = false;
		this.root = root;
		this.name = name;

		ReadElements(reader);
	}


	/// <summary>
	/// Reads the xml elements from the reader and stores them in the hashtable
	/// </summary>
	/// <param name="reader"></param>
	private void ReadElements(Xml.XmlTextReader reader)
	{
		try
		{
			while (reader.Read())
			{
				switch (reader.NodeType)
				{
					case Xml.XmlNodeType.Element:

						// we've hit a value, so read it in
						if (reader.Name == keyName)
						{
							string name = reader.GetAttribute("name");
							if (name == string.Empty || name == "")
								throw new ApplicationException();

							if (!reader.IsEmptyElement && reader.Read())
							{
//								System.Diagnostics.Debug.WriteLine("Read: " + this.name + " " + name + " = " + reader.Value);
								this.keyTable.Add(name, reader.Value);
							}
							else
								this.keyTable.Add(name, string.Empty);
						}
							// hit a subsection, read in the name of it
						else if (reader.Name == subsectionName)
						{
							if (!reader.IsEmptyElement)
							{
								string name = reader.GetAttribute("name");
								if (name == string.Empty || name == "")
									throw new ApplicationException();

								this.subsectionTable.Add(name, new XmlStorage(this.root, name, reader));
							}
						}

						break;

					case Xml.XmlNodeType.EndElement:

						// end of subsection, party is over
						if (reader.Name == subsectionName && !this.isRoot)
							return;
						break;
				}
			}
		}
		catch (Exception e)
		{
			throw new ApplicationException("Malformed xml storage file", e);
		}
	}


	/// <summary>
	/// Constructs an XmlStorage class
	/// </summary>
	/// <param name="fileName">Xml file to use</param>
	public XmlStorage(string fileName)
	{
		this.isRoot = true;
		this.root = this;
		this.fileName = fileName;
		this.name = "Root";

		try
		{
			IO.FileStream stream = new IO.FileStream(fileName, IO.FileMode.Open, IO.FileAccess.Read);
			Xml.XmlTextReader reader = new Xml.XmlTextReader(stream);

			try
			{
				this.ReadElements(reader);
			}
			finally
			{
				reader.Close();
				stream.Close();
			}
		}
		catch (IO.FileNotFoundException)
		{}
	}


	#region Close methods


	/// <summary>
	/// Closes the Xml file
	/// </summary>
	public void Close()
	{
		Flush();
	}


	/// <summary>
	/// Flushes the hashtable to the xml file
	/// </summary>
	public void Flush()
	{
		if (isRoot)
		{
			Xml.XmlTextWriter writer = new Xml.XmlTextWriter(this.fileName, System.Text.Encoding.ASCII);

			try
			{
	//			writer.Formatting = Xml.Formatting.Indented;

				writer.WriteStartDocument(true);
				writer.WriteStartElement(rootName);

				WriteElements(writer);

				writer.WriteEndElement();
			}
			finally
			{
				writer.Close();
			}
		}
	}


	/// <summary>
	/// Writes elements to an xml writer
	/// </summary>
	/// <param name="writer"></param>
	private void WriteElements(Xml.XmlTextWriter writer)
	{
		// iterate across all keys in this section and write them to the xml file
		IDictionaryEnumerator enumerator = this.keyTable.GetEnumerator();

		while (enumerator.MoveNext())
		{
			writer.WriteStartElement(keyName);
			writer.WriteAttributeString("name", (string)enumerator.Key);
			writer.WriteString((string)enumerator.Value);

//			System.Diagnostics.Debug.WriteLine(
//"Wrote: " + this.name + " " + (string)enumerator.Key + " = " + (string)enumerator.Value);

			writer.WriteEndElement();
		}

		// then do it for all sub sections
		foreach (XmlStorage xml in this.subsectionTable.Values)
		{
			writer.WriteStartElement(subsectionName);
			writer.WriteAttributeString("name", xml.name);

			xml.WriteElements(writer);

			writer.WriteEndElement();
		}
	}


	#endregion


	#region Sub-section methods


	/// <summary>
	/// Creates a subsection
	/// </summary>
	IStorage IStorage.CreateSubSection(string name)
	{
		return this.CreateSubSection(name);
	}


	/// <summary>
	/// Creates an Xml subsection
	/// </summary>
	/// <param name="name">Name of subsection</param>
	/// <returns></returns>
	public XmlStorage CreateSubSection(string name)
	{
		if (this.subsectionTable.Contains(name))
			return (XmlStorage)this.subsectionTable[name];
		else
		{
			XmlStorage sub = new XmlStorage(this.root, name);
			this.subsectionTable.Add(name, sub);
			return sub;
		}
	}


	#endregion


	#region Write methods


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	public void Write(string name, bool val)
	{
		if (!this.keyTable.Contains(name))
			this.keyTable.Add(name, Xml.XmlConvert.ToString(val));
		else
			this.keyTable[name] = Xml.XmlConvert.ToString(val);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	public void Write(string name, char val)
	{
		if (!this.keyTable.Contains(name))
			this.keyTable.Add(name, Xml.XmlConvert.ToString(val));
		else
			this.keyTable[name] = Xml.XmlConvert.ToString(val);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	public void Write(string name, byte val)
	{
		if (!this.keyTable.Contains(name))
			this.keyTable.Add(name, Xml.XmlConvert.ToString(val));
		else
			this.keyTable[name] = Xml.XmlConvert.ToString(val);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	public void Write(string name, string val)
	{
		if (val == string.Empty || val == "")
			val = "";

		if (!this.keyTable.Contains(name))
			this.keyTable.Add(name, val);
		else
			this.keyTable[name] = val;
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	public void Write(string name, Single val)
	{
		if (!this.keyTable.Contains(name))
			this.keyTable.Add(name, Xml.XmlConvert.ToString(val));
		else
			this.keyTable[name] = Xml.XmlConvert.ToString(val);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	public void Write(string name, Double val)
	{
		if (!this.keyTable.Contains(name))
			this.keyTable.Add(name, Xml.XmlConvert.ToString(val));
		else
			this.keyTable[name] = Xml.XmlConvert.ToString(val);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	public void Write(string name, Int16 val)
	{
		if (!this.keyTable.Contains(name))
			this.keyTable.Add(name, Xml.XmlConvert.ToString(val));
		else
			this.keyTable[name] = Xml.XmlConvert.ToString(val);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	public void Write(string name, Int32 val)
	{
		if (!this.keyTable.Contains(name))
			this.keyTable.Add(name, Xml.XmlConvert.ToString(val));
		else
			this.keyTable[name] = Xml.XmlConvert.ToString(val);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	public void Write(string name, Int64 val)
	{
		if (!this.keyTable.Contains(name))
			this.keyTable.Add(name, Xml.XmlConvert.ToString(val));
		else
			this.keyTable[name] = Xml.XmlConvert.ToString(val);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	public void Write(string name, UInt16 val)
	{
		if (!this.keyTable.Contains(name))
			this.keyTable.Add(name, Xml.XmlConvert.ToString(val));
		else
			this.keyTable[name] = Xml.XmlConvert.ToString(val);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	public void Write(string name, UInt32 val)
	{
		if (!this.keyTable.Contains(name))
			this.keyTable.Add(name, Xml.XmlConvert.ToString(val));
		else
			this.keyTable[name] = Xml.XmlConvert.ToString(val);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	public void Write(string name, UInt64 val)
	{
		if (!this.keyTable.Contains(name))
			this.keyTable.Add(name, Xml.XmlConvert.ToString(val));
		else
			this.keyTable[name] = Xml.XmlConvert.ToString(val);
	}


	#endregion


	#region Read methods


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="def"></param>
	/// <returns></returns>
	public bool ReadBool(string name, bool def)
	{
		if (!this.keyTable.Contains(name))
		{
			Write(name, def);
			return def;
		}
		else
			return Xml.XmlConvert.ToBoolean((string)this.keyTable[name]);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="def"></param>
	/// <returns></returns>
	public char ReadChar(string name, char def)
	{
		if (!this.keyTable.Contains(name))
		{
			Write(name, def);
			return def;
		}
		else
			return Xml.XmlConvert.ToChar((string)this.keyTable[name]);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="def"></param>
	/// <returns></returns>
	public byte ReadByte(string name, byte def)
	{
		if (!this.keyTable.Contains(name))
		{
			Write(name, def);
			return def;
		}
		else
			return Xml.XmlConvert.ToByte((string)this.keyTable[name]);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="def"></param>
	/// <returns></returns>
	public string ReadString(string name, string def)
	{
		if (!this.keyTable.Contains(name))
		{
			Write(name, def);
			return def;
		}
		else
			return (string)this.keyTable[name];
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="def"></param>
	/// <returns></returns>
	public Single ReadSingle(string name, Single def)
	{
		if (!this.keyTable.Contains(name))
		{
			Write(name, def);
			return def;
		}
		else
			return Xml.XmlConvert.ToSingle((string)this.keyTable[name]);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="def"></param>
	/// <returns></returns>
	public Double ReadDouble(string name, Double def)
	{
		if (!this.keyTable.Contains(name))
		{
			Write(name, def);
			return def;
		}
		else
			return Xml.XmlConvert.ToDouble((string)this.keyTable[name]);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="def"></param>
	/// <returns></returns>
	public Int16 ReadInt16(string name, Int16 def)
	{
		if (!this.keyTable.Contains(name))
		{
			Write(name, def);
			return def;
		}
		else
			return Xml.XmlConvert.ToInt16((string)this.keyTable[name]);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="def"></param>
	/// <returns></returns>
	public Int32 ReadInt32(string name, Int32 def)
	{
		if (!this.keyTable.Contains(name))
		{
			Write(name, def);
			return def;
		}
		else
			return Xml.XmlConvert.ToInt32((string)this.keyTable[name]);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="def"></param>
	/// <returns></returns>
	public Int64 ReadInt64(string name, Int64 def)
	{
		if (!this.keyTable.Contains(name))
		{
			Write(name, def);
			return def;
		}
		else
			return Xml.XmlConvert.ToInt64((string)this.keyTable[name]);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="def"></param>
	/// <returns></returns>
	public UInt16 ReadUInt16(string name, UInt16 def)
	{
		if (!this.keyTable.Contains(name))
		{
			Write(name, def);
			return def;
		}
		else
			return Xml.XmlConvert.ToUInt16((string)this.keyTable[name]);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="def"></param>
	/// <returns></returns>
	public UInt32 ReadUInt32(string name, UInt32 def)
	{
		if (!this.keyTable.Contains(name))
		{
			Write(name, def);
			return def;
		}
		else
			return Xml.XmlConvert.ToUInt32((string)this.keyTable[name]);
	}


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="def"></param>
	/// <returns></returns>
	public UInt64 ReadUInt64(string name, UInt64 def)
	{
		if (!this.keyTable.Contains(name))
		{
			Write(name, def);
			return def;
		}
		else
			return Xml.XmlConvert.ToUInt64((string)this.keyTable[name]);
	}


	#endregion

}
