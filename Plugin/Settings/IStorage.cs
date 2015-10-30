using System;

// 31/5/03


/// <summary>
/// Base interface for storing key-value settings.
/// </summary>
public interface IStorage
{
	#region Close methods


	/// <summary>
	/// Closes the storage medium
	/// </summary>
	void Close();


	/// <summary>
	/// Flushes current data (if buffered)
	/// </summary>
	void Flush();


	#endregion


	#region Sub-section methods


	/// <summary>
	/// Creates a subsection - throws an UnsupportedException() if not supported.
	/// </summary>
	IStorage CreateSubSection(string name);


	#endregion


	#region Write methods


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	void Write(string name, bool val);


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	void Write(string name, char val);


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	void Write(string name, byte val);

	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	void Write(string name, string val);


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	void Write(string name, Single val);

	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	void Write(string name, Double val);


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	void Write(string name, Int16 val);


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	void Write(string name, Int32 val);


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	void Write(string name, Int64 val);


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	void Write(string name, UInt16 val);


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	void Write(string name, UInt32 val);


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="val"></param>
	void Write(string name, UInt64 val);


	#endregion


	#region Read methods


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="def"></param>
	/// <returns></returns>
	bool ReadBool(string name, bool def);

	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="def"></param>
	/// <returns></returns>
	char ReadChar(string name, char def);


	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="def"></param>
	/// <returns></returns>
	byte ReadByte(string name, byte def);

	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="name"></param>
	/// <param name="def"></param>
	/// <returns></returns>
	string ReadString(string name, string def);


	/// <summary>
	/// 
	/// </summary>
	Single ReadSingle(string name, Single def);


	/// <summary>
	/// 
	/// </summary>
	Double ReadDouble(string name, Double def);


	/// <summary>
	/// 
	/// </summary>
	Int16 ReadInt16(string name, Int16 def);

	
	/// <summary>
	/// 
	/// </summary>
	Int32 ReadInt32(string name, Int32 def);


	/// <summary>
	/// 
	/// </summary>
	Int64 ReadInt64(string name, Int64 def);


	/// <summary>
	/// 
	/// </summary>
	UInt16 ReadUInt16(string name, UInt16 def);

	
	/// <summary>
	/// 
	/// </summary>
	UInt32 ReadUInt32(string name, UInt32 def);


	/// <summary>
	/// 
	/// </summary>
	UInt64 ReadUInt64(string name, UInt64 def);


	#endregion

}

