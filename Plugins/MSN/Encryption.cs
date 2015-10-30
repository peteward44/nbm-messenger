using System;

using Crypto = System.Security.Cryptography;


public interface IEncryptionMethod
{
	string Encrypt(string hash);

	string Name
	{ get; }
}


public class MD5Encryption : IEncryptionMethod
{
	public string Name
	{ get { return "MD5"; } }

	
	public string Encrypt(string hash)
	{
		Crypto.MD5CryptoServiceProvider md5 = new Crypto.MD5CryptoServiceProvider();

		byte[] bytes = md5.ComputeHash(System.Text.Encoding.ASCII.GetBytes(hash));

		string finished = string.Empty;

		foreach (byte b in bytes)
		{
			finished += string.Format("{0:x2}", (int)b);
		}

		return finished;
	}
}

