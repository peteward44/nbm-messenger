using System;
using Win32 = Microsoft.Win32;
using WinForms = System.Windows.Forms;

// 31/5/03

namespace NBM.Plugin
{
	/// <summary>
	/// Stores settings in the registry.
	/// </summary>
	public class RegistryStorage : IStorage
	{
		private Win32.RegistryKey regKey;


		public RegistryStorage(Win32.RegistryKey rootKey, string subkey)
		{
			this.regKey = rootKey.CreateSubKey(subkey);
		}


		public RegistryStorage(bool perUserSettings)
		{
			if (perUserSettings)
				this.regKey = WinForms.Application.UserAppDataRegistry;
			else
				this.regKey = WinForms.Application.CommonAppDataRegistry;
		}


		#region Close methods


		public void Close()
		{
			this.regKey.Close();
		}

		public void Flush()
		{
			this.regKey.Flush();
		}


		#endregion


		#region Sub-section methods


		public IStorage CreateSubSection(string name)
		{
			return new RegistryStorage(this.regKey, name);
		}


		#endregion


		#region Write methods

		public void Write(string name, bool val)
		{
			this.regKey.SetValue(name, val ? 1 : 0);
		}

		public void Write(string name, char val)
		{
			this.regKey.SetValue(name, val);
		}

		public void Write(string name, byte val)
		{
			this.regKey.SetValue(name, val);
		}

		public void Write(string name, string val)
		{
			this.regKey.SetValue(name, val);
		}

		public void Write(string name, byte[] val)
		{
			this.regKey.SetValue(name, val);
		}

		public void Write(string name, char[] val)
		{
			this.regKey.SetValue(name, new string(val));
		}

		public void Write(string name, Single val)
		{
			this.regKey.SetValue(name, val);
		}

		public void Write(string name, Double val)
		{
			this.regKey.SetValue(name, val);
		}

		public void Write(string name, Int16 val)
		{
			this.regKey.SetValue(name, val);
		}

		public void Write(string name, Int32 val)
		{
			this.regKey.SetValue(name, val);
		}

		public void Write(string name, Int64 val)
		{
			this.regKey.SetValue(name, val);
		}

		public void Write(string name, UInt16 val)
		{
			this.regKey.SetValue(name, val);
		}

		public void Write(string name, UInt32 val)
		{
			this.regKey.SetValue(name, val);
		}

		public void Write(string name, UInt64 val)
		{
			this.regKey.SetValue(name, val);
		}


		#endregion


		#region Read methods


		public bool ReadBool(string name, bool def)
		{
			return (int)this.regKey.GetValue(name, def ? 1 : 0) == 1;
		}

		public char ReadChar(string name, char def)
		{
			return ((string)this.regKey.GetValue(name, def))[0];
		}

		public byte ReadByte(string name, byte def)
		{
			return byte.Parse((string)this.regKey.GetValue(name, def));
		}

		public string ReadString(string name, string def)
		{
			return (string)this.regKey.GetValue(name, def);
		}


		public byte[] ReadBytes(string name, byte[] def)
		{
			return (byte[])this.regKey.GetValue(name, def);
		}

		public char[] ReadChars(string name, char[] def)
		{
			return ((string)this.regKey.GetValue(name, new string(def))).ToCharArray();
		}


		public byte[] ReadBytes(string name)
		{
			return (byte[])this.regKey.GetValue(name);
		}

		public char[] ReadChars(string name)
		{
			return ((string)this.regKey.GetValue(name)).ToCharArray();
		}


		public Single ReadSingle(string name, Single def)
		{
			return Single.Parse((string)this.regKey.GetValue(name, def));
		}

		public Double ReadDouble(string name, Double def)
		{
			return Double.Parse((string)this.regKey.GetValue(name, def));
		}


		public Int16 ReadInt16(string name, Int16 def)
		{
			return Int16.Parse((string)this.regKey.GetValue(name, def));
		}

		public Int32 ReadInt32(string name, Int32 def)
		{
			return (Int32)this.regKey.GetValue(name, def);
		}

		public Int64 ReadInt64(string name, Int64 def)
		{
			return Int64.Parse((string)this.regKey.GetValue(name, def));
		}


		public UInt16 ReadUInt16(string name, UInt16 def)
		{
			return UInt16.Parse((string)this.regKey.GetValue(name, def));
		}

		public UInt32 ReadUInt32(string name, UInt32 def)
		{
			return UInt32.Parse((string)this.regKey.GetValue(name, def));
		}

		public UInt64 ReadUInt64(string name, UInt64 def)
		{
			return UInt64.Parse((string)this.regKey.GetValue(name, def));
		}


		#endregion
	}
}
