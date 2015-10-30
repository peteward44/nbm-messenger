using System;
using System.Collections;
using System.Text;

using IO = System.IO;
using Crypto = System.Security.Cryptography;

// 18/5/03

public class Encryption
{
	private Encryption()
	{
	}


	public static void Encrypt(string username, string password, byte[] salt, out byte[] result1, out byte[] result2)
	{
		byte[] byteUsername = ASCIIEncoding.ASCII.GetBytes(username);
		byte[] bytePassword = ASCIIEncoding.ASCII.GetBytes(password);

		// ok, heres the dirty stuff.
		// take the 16th character from the yahoo seed string and take the modulus of it
		int sv = (int)salt[15];
		sv = (sv % 8) % 5;

		// md5 encrypt the password
		Crypto.MD5CryptoServiceProvider md5 = new Crypto.MD5CryptoServiceProvider();

		byte[] passwordResult = ToMac64(md5.ComputeHash(bytePassword));
		byte[] cryptResult = ToMac64(md5.ComputeHash(YahooCrypt(password, "_2S43d5f")));

		// the order of the strings depends on the value of sv (calculated above)
		byte checksum;

		IO.MemoryStream result1Stream = new IO.MemoryStream();
		IO.MemoryStream result2Stream = new IO.MemoryStream();

		switch (sv) 
		{
			case 0:
				checksum = salt[salt[7] % 16];

				result1Stream.WriteByte(checksum);
				result1Stream.Write(passwordResult, 0, passwordResult.Length);
				result1Stream.Write(byteUsername, 0, byteUsername.Length);
				result1Stream.Write(salt, 0, salt.Length);

				result2Stream.WriteByte(checksum);
				result2Stream.Write(cryptResult, 0, cryptResult.Length);
				result2Stream.Write(byteUsername, 0, byteUsername.Length);
				result2Stream.Write(salt, 0, salt.Length);

				break;
			case 1:
				checksum = salt[salt[9] % 16];

				result1Stream.WriteByte(checksum);
				result1Stream.Write(byteUsername, 0, byteUsername.Length);
				result1Stream.Write(salt, 0, salt.Length);
				result1Stream.Write(passwordResult, 0, passwordResult.Length);

				result2Stream.WriteByte(checksum);
				result2Stream.Write(byteUsername, 0, byteUsername.Length);
				result2Stream.Write(salt, 0, salt.Length);
				result2Stream.Write(cryptResult, 0, cryptResult.Length);

				break;
			case 2:
				checksum = salt[salt[15] % 16];

				result1Stream.WriteByte(checksum);
				result1Stream.Write(salt, 0, salt.Length);
				result1Stream.Write(passwordResult, 0, passwordResult.Length);
				result1Stream.Write(byteUsername, 0, byteUsername.Length);

				result2Stream.WriteByte(checksum);
				result2Stream.Write(salt, 0, salt.Length);
				result2Stream.Write(cryptResult, 0, cryptResult.Length);
				result2Stream.Write(byteUsername, 0, byteUsername.Length);

				break;
			case 3:
				checksum = salt[salt[1] % 16];

				result1Stream.WriteByte(checksum);
				result1Stream.Write(byteUsername, 0, byteUsername.Length);
				result1Stream.Write(passwordResult, 0, passwordResult.Length);
				result1Stream.Write(salt, 0, salt.Length);

				result2Stream.WriteByte(checksum);
				result2Stream.Write(byteUsername, 0, byteUsername.Length);
				result2Stream.Write(cryptResult, 0, cryptResult.Length);
				result2Stream.Write(salt, 0, salt.Length);

				break;
			case 4:
				checksum = salt[salt[3] % 16];

				result1Stream.WriteByte(checksum);
				result1Stream.Write(passwordResult, 0, passwordResult.Length);
				result1Stream.Write(salt, 0, salt.Length);
				result1Stream.Write(byteUsername, 0, byteUsername.Length);

				result2Stream.WriteByte(checksum);
				result2Stream.Write(cryptResult, 0, cryptResult.Length);
				result2Stream.Write(salt, 0, salt.Length);
				result2Stream.Write(byteUsername, 0, byteUsername.Length);

				break;
		}

		result1 = ToMac64(md5.ComputeHash(result1Stream.ToArray()), 0, 16);
		result2 = ToMac64(md5.ComputeHash(result2Stream.ToArray()), 0, 16);

		result1Stream.Close();
		result2Stream.Close();
	}


	public static byte[] YahooCrypt(string key, string salt)
	{
		const string md5SaltPrefix = "$1$";

		byte[] byteSaltPrefix = ASCIIEncoding.ASCII.GetBytes(md5SaltPrefix);
		byte[] byteKey = ASCIIEncoding.ASCII.GetBytes(key);
		byte[] byteSalt = ASCIIEncoding.ASCII.GetBytes(salt);

		// create a memory stream for the result
		IO.MemoryStream result = new IO.MemoryStream();

		result.Write(byteKey, 0, byteKey.Length);
		result.Write(byteSaltPrefix, 0, byteSaltPrefix.Length);
		result.Write(byteSalt, 0, byteSalt.Length);

		// create an alternate string for encyption
		IO.MemoryStream altResult = new IO.MemoryStream();

		altResult.Write(byteKey, 0, byteKey.Length);
		altResult.Write(byteSalt, 0, byteSalt.Length);
		altResult.Write(byteKey, 0, byteKey.Length);

		// encrypt alternate result
		Crypto.MD5CryptoServiceProvider md5 = new Crypto.MD5CryptoServiceProvider();
		byte[] altResultHash = md5.ComputeHash(altResult.ToArray());

		// Add for any character in the key one byte of the alternate sum.
		int cnt;
		for (cnt = byteKey.Length; cnt > 16; cnt -= 16)
			result.Write(altResultHash, 0, 16);
		result.Write(altResultHash, 0, cnt);

		/* For the following code we need a NUL byte.  */
		altResultHash[0] = 0;

		/* The original implementation now does something weird: for every 1
			 bit in the key the first 0 is added to the buffer, for every 0
			 bit the first character of the key.  This does not seem to be
			 what was intended but we have to follow this to be compatible.  */
		for (cnt = key.Length; cnt > 0; cnt >>= 1)
			result.Write( ((cnt & 1) != 0 ? altResultHash : byteKey), 0, 1);

		// create intermediate result
		altResultHash = md5.ComputeHash(result.ToArray());

		/* Now comes another weirdness.  In fear of password crackers here
			 comes a quite long loop which just processes the output of the
			 previous round again.  We cannot ignore this here.  */
		for (cnt = 0; cnt < 1000; ++cnt) 
		{
			result.Seek(0, IO.SeekOrigin.Begin);
			result.SetLength(0);

			/* Add key or last result.  */
			if ((cnt & 1) != 0)
				result.Write(byteKey, 0, byteKey.Length);
			else
				result.Write(altResultHash, 0, 16);

			/* Add salt for numbers not divisible by 3.  */
			if (cnt % 3 != 0)
				result.Write(byteSalt, 0, byteSalt.Length);

			/* Add key for numbers not divisible by 7.  */
			if (cnt % 7 != 0)
				result.Write(byteKey, 0, byteKey.Length);

			/* Add key or last result.  */
			if ((cnt & 1) != 0)
				result.Write(altResultHash, 0, 16);
			else
				result.Write(byteKey, 0, byteKey.Length);

			/* Create intermediate result.  */
			altResultHash = md5.ComputeHash(result.ToArray());
		}

		/* Now we can construct the result string.  It consists of three
				 parts.  */

		// start with the salt prefix
		IO.MemoryStream finalResult = new IO.MemoryStream();

		finalResult.Write(byteSaltPrefix, 0, byteSaltPrefix.Length);
		finalResult.Write(byteSalt, 0, byteSalt.Length);
		finalResult.WriteByte((byte)'$');

		b64_from_24bit (altResultHash[0], altResultHash[6], altResultHash[12], 4, finalResult);
		b64_from_24bit (altResultHash[1], altResultHash[7], altResultHash[13], 4, finalResult);
		b64_from_24bit (altResultHash[2], altResultHash[8], altResultHash[14], 4, finalResult);
		b64_from_24bit (altResultHash[3], altResultHash[9], altResultHash[15], 4, finalResult);
		b64_from_24bit (altResultHash[4], altResultHash[10], altResultHash[5], 4, finalResult);
		b64_from_24bit (0, 0, altResultHash[11], 2, finalResult);

		result.Close();
		altResult.Close();

		byte[] done = finalResult.ToArray();

		finalResult.Close();

		return done;
	}


	private static void b64_from_24bit(byte b2, byte b1, byte b0, int n, IO.MemoryStream finalResult)
	{
		const string b64t = "./0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

		int w = ((b2) << 16) | ((b1) << 8) | (b0);
		while (n-- > 0) 
		{
			finalResult.WriteByte((byte)b64t[w & 0x3f]);
			w >>= 6;
		}
	}


	public static byte[] ToMac64(byte[] str)
	{
		return ToMac64(str, 0, str.Length);
	}


	public static byte[] ToMac64(byte[] str, int index, int length)
	{
		string stringBase64digits = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789._";
		byte[] base64digits = ASCIIEncoding.ASCII.GetBytes(stringBase64digits);

		IO.MemoryStream result = new IO.MemoryStream();

		/* raw bytes in quasi-big-endian order to base 64 string (NUL-terminated) */
		int inputStringPosition = index;
		int inputStringLength = length;

		for (; inputStringLength >= 3; inputStringLength -= 3)
		{
			result.WriteByte(base64digits[str[inputStringPosition] >> 2]);
			result.WriteByte(base64digits[((str[inputStringPosition]<<4) & 0x30) | (str[inputStringPosition+1]>>4)]);
			result.WriteByte(base64digits[((str[inputStringPosition+1]<<2) & 0x3c) | (str[inputStringPosition+2]>>6)]);
			result.WriteByte(base64digits[str[inputStringPosition+2] & 0x3f]);
			inputStringPosition += 3;
		}

		if (inputStringLength > 0)
		{
			int fragment;

			result.WriteByte(base64digits[str[inputStringPosition] >> 2]);
			fragment = (str[inputStringPosition] << 4) & 0x30;

			if (inputStringLength > 1)
				fragment |= str[inputStringPosition+1] >> 4;

			result.WriteByte(base64digits[fragment]);
			result.WriteByte((inputStringLength < 2) ? (byte)'-' : base64digits[(str[inputStringPosition+1] << 2) & 0x3c]);
			result.WriteByte((byte)'-');
		}

		byte[] final = result.ToArray();

		result.Close();
		
		return final;
	}
}

