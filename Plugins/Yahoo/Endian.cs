using System;

// 17/5/03

public class Endian
{
	private Endian()
	{}


	public static Int64 Convert(Int64 i)
	{
		return ((Int64)Convert((Int32)(i % 0x100000000)) * 0x100000000) + (Int64)Convert((Int32)(i / 0x100000000));
	}


	public static Int32 Convert(Int32 i)
	{
		return ((Int32)Convert((Int16)(i % 0x10000))) * 0x10000 + (Int32)Convert((Int16)(i / 0x10000));
	}


	public static Int16 Convert(Int16 i)
	{
		return (Int16)(((i % 0x100) * 0x100) + (i / 0x100));
	}


	public static UInt64 Convert(UInt64 i)
	{
		return ((UInt64)Convert((UInt32)(i % 0x100000000)) * 0x100000000) + (UInt64)Convert((UInt32)(i / 0x100000000));
	}


	public static UInt32 Convert(UInt32 i)
	{
		return ((UInt32)Convert((UInt16)(i % 0x10000))) * 0x10000 + (UInt32)Convert((UInt16)(i / 0x10000));
	}


	public static UInt16 Convert(UInt16 i)
	{
		return (UInt16)(((i % 0x100) * 0x100) + (i / 0x100));
	}


	public static readonly bool IsLittleEndian = System.BitConverter.IsLittleEndian;
}

