using System;
using System.Collections;
using NBM.Plugin;

// 9/6/03

/// <summary>
/// Summary description for Constants.
/// </summary>
public class Constants : IConstants
{
	public Constants()
	{
	}


	public bool SupportsFileTransfer
	{
		get { return true; }
	}

	public bool ReasonSentWithAddFriend
	{
		get { return false; }
	}


	public string Name
	{
		get { return "MSN"; }
	}

	public string DefaultServerHost
	{
		get { return "messenger.hotmail.com"; }
	}

	public int DefaultServerPort
	{
		get { return 1863; }
	}
}
