using System;
using System.Runtime.InteropServices;

using WinForms = System.Windows.Forms;

// 31/5/03

namespace NBM.Plugin
{
	/// <summary>
	/// Stores settings in an INI file.
	/// </summary>
	public class IniStorage : IStorage
	{
		#region Imported INI functions

		[DllImport("kernel32.dll")]
		private static unsafe extern bool WritePrivateProfileStruct(string sectionName, string keyName, void* data, uint dataSize, string fileName);

		[DllImport("kernel32.dll")]
		private static unsafe extern bool WritePrivateProfileString(string sectionName, string keyName, byte* data, string fileName);

		[DllImport("kernel32.dll")]
		private static extern UInt32 GetPrivateProfileInt(string sectionName, string keyName, uint def, string fileName);

		[DllImport("kernel32.dll")]
		private static unsafe extern UInt32 GetPrivateProfileString(string sectionName, string keyName, string defString, byte* destination, uint size, string fileName);

		[DllImport("kernel32.dll")]
		private static unsafe extern bool GetPrivateProfileStruct(string sectionName, string keyName, byte* data, uint dataSize, string fileName);


		#endregion



		private const string lengthPostFix = "Length__";
		private const int maxStringSize = 1024;
		private const string sectionName = "Settings";

		private string fileName;


		public IniStorage(string fileName, bool perUserSettings)
		{
			if (fileName.StartsWith(new string(new char[] { System.IO.Path.DirectorySeparatorChar })))
				fileName = fileName.Substring(1);

			if (perUserSettings)
			{
				if (fileName[1] != ':')
				{
					// its not an absolute path, so put it in the user app data path.
					fileName = WinForms.Application.UserAppDataPath + fileName;
				}
				else
				{
					// it is an absolute path, so append the current username to the directory hierarchy.
					int pos = fileName.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
					string path = fileName.Substring(0, pos);
					string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
					int pos2 = username.LastIndexOf(@"\");
					if (pos2 > 0)
						username = username.Substring(pos2+1);
					path += System.IO.Path.DirectorySeparatorChar + username;

					System.IO.Directory.CreateDirectory(path);

					fileName = path + System.IO.Path.DirectorySeparatorChar + fileName.Substring(pos+1);
				}
			}
			else
			{
				// bug fix: if the file is not an absolute path, then it'll put it in the windows
				// directory. Make the file path relative to the common app data path.
				if (fileName[1] != ':')
				{
					fileName = WinForms.Application.CommonAppDataPath + fileName;
				}
			}

			this.fileName = fileName;
		}


		private IniStorage(string fileName, string sectionName)
		{
			// calculate new filename
			int pos = fileName.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
			string path = fileName.Substring(0, pos);
			path += System.IO.Path.DirectorySeparatorChar + sectionName;

			System.IO.Directory.CreateDirectory(path);

			path += System.IO.Path.DirectorySeparatorChar + fileName.Substring(pos+1);

			this.fileName = path;
		}


		#region Close methods


		public void Close()
		{
		}


		public void Flush()
		{
		}


		#endregion


		#region Sub-section methods

		public IStorage CreateSubSection(string name)
		{
			return new IniStorage(this.fileName, name);
		}

		#endregion


		#region Write methods


		private unsafe void WriteASCIIString(string name, string val)
		{
			byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(val);

			fixed (byte* bp = b)
			{
				WritePrivateProfileString(sectionName, name, bp, this.fileName);
			}
		}


		private void WriteUNICODEString(string name, string val)
		{
			byte[] b = System.Text.UnicodeEncoding.Unicode.GetBytes(val);

			// then write the unicode string as a series of bytes
			Write(name, b);
		}


		private unsafe void WriteBytes(string name, byte[] b)
		{
			// write length
			Write(name + lengthPostFix, b.Length);

			fixed (byte* pos = b)
			{
				WritePrivateProfileStruct(sectionName, name, pos, (uint)b.Length, this.fileName);
			}
		}


		public void Write(string name, string val)
		{
			WriteUNICODEString(name, val);
		}

		public void Write(string name, byte[] val)
		{
			WriteBytes(name, val);
		}

		public void Write(string name, bool val)
		{
			Write(name, val ? 1 : 0);
		}

		public void Write(string name, char val)
		{
			Write(name, (Int32)val);
		}

		public void Write(string name, byte val)
		{
			Write(name, (Int32)val);
		}

		public void Write(string name, char[] val)
		{
			Write(name, new string(val));
		}

		public void Write(string name, Single val)
		{
			WriteASCIIString(name, val.ToString());
		}

		public void Write(string name, Double val)
		{
			WriteASCIIString(name, val.ToString());
		}

		public void Write(string name, Int16 val)
		{
			WriteASCIIString(name, val.ToString());
		}

		public void Write(string name, Int32 val)
		{
			WriteASCIIString(name, val.ToString());
		}

		public void Write(string name, Int64 val)
		{
			WriteASCIIString(name, val.ToString());
		}

		public void Write(string name, UInt16 val)
		{
			WriteASCIIString(name, val.ToString());
		}

		public void Write(string name, UInt32 val)
		{
			WriteASCIIString(name, val.ToString());
		}

		public void Write(string name, UInt64 val)
		{
			WriteASCIIString(name, val.ToString());
		}


		#endregion


		#region Read methods


		private unsafe string ReadASCIIString(string name, string def)
		{
			byte[] b = new byte[maxStringSize];

			fixed (byte* des = b)
			{
				GetPrivateProfileString(sectionName, name, def, des, maxStringSize, this.fileName);
			}

			return System.Text.ASCIIEncoding.ASCII.GetString(b);
		}


		private unsafe string ReadUNICODEString(string name, string def)
		{
			// read length of string from length field
			uint length = this.ReadUInt32(name + "Length__", maxStringSize);

			byte[] b = new byte[length];

			fixed (byte* des = b)
			{
				if (!GetPrivateProfileStruct(sectionName, name, des, length, this.fileName))
					return def;
			}

			return System.Text.UnicodeEncoding.Unicode.GetString(b);
		}


		private unsafe byte[] pReadBytes(string name, byte[] def)
		{
			// read length of string from length field
			uint length = this.ReadUInt32(name + "Length__", maxStringSize);

			byte[] b = new byte[length];

			fixed (byte* des = b)
			{
				if (!GetPrivateProfileStruct(sectionName, name, des, length, this.fileName))
					return def;
			}

			return b;
		}



		public bool ReadBool(string name, bool def)
		{
			return ReadInt32(name, def ? 1 : 0) == 1;
		}

		public char ReadChar(string name, char def)
		{
			return (char)ReadInt32(name, (Int32)def);
		}

		public byte ReadByte(string name, byte def)
		{
			return (byte)ReadInt32(name, (Int32)def);
		}

		public string ReadString(string name, string def)
		{
			return ReadUNICODEString(name, def);
		}


		public byte[] ReadBytes(string name, byte[] def)
		{
			return pReadBytes(name, def);
		}


		public char[] ReadChars(string name, char[] def)
		{
			return ReadUNICODEString(name, new string(def)).ToCharArray();
		}


		public byte[] ReadBytes(string name)
		{
			return ReadBytes(name, null);
		}

		public char[] ReadChars(string name)
		{
			return ReadChars(name, null);
		}


		public Single ReadSingle(string name, Single def)
		{
			return Single.Parse(ReadASCIIString(name, def.ToString()));
		}

		public Double ReadDouble(string name, Double def)
		{
			return Double.Parse(ReadASCIIString(name, def.ToString()));
		}


		public Int16 ReadInt16(string name, Int16 def)
		{
			return Int16.Parse(ReadASCIIString(name, def.ToString()));
		}

		public Int32 ReadInt32(string name, Int32 def)
		{
			return Int32.Parse(ReadASCIIString(name, def.ToString()));
		}

		public Int64 ReadInt64(string name, Int64 def)
		{
			return Int64.Parse(ReadASCIIString(name, def.ToString()));
		}


		public UInt16 ReadUInt16(string name, UInt16 def)
		{
			return UInt16.Parse(ReadASCIIString(name, def.ToString()));
		}

		public UInt32 ReadUInt32(string name, UInt32 def)
		{
			return UInt32.Parse(ReadASCIIString(name, def.ToString()));
		}

		public UInt64 ReadUInt64(string name, UInt64 def)
		{
			return UInt64.Parse(ReadASCIIString(name, def.ToString()));
		}


		#endregion
	}
}
