using System;
using NBM.Plugin;

// 14/6/03

public class Constants : IConstants
{
	public Constants()
	{
	}


	public bool SupportsFileTransfer
	{
		get { return false; }
	}


	public bool ReasonSentWithAddFriend
	{
		get { return true; }
	}


	public string Name
	{
		get { return "Yahoo"; }
	}

	public string DefaultServerHost
	{
		get { return "scs.yahoo.com"; }
	}

	public int DefaultServerPort
	{
		get { return 5050; }
	}
}
