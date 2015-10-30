using System;
using System.Collections;
using NBM.Plugin;

using Xml = System.Xml;
using IO = System.IO;

using System.Runtime.Serialization.Formatters.Binary;

// 9/6/03

/// <summary>
/// Summary description for Settings.
/// </summary>
public class Settings : ProtocolSettings
{
	private IStorage storage;

	private int contactListVersion = 0;
	private ArrayList friendsAllowList = new ArrayList();
	private ArrayList friendsReverseList = new ArrayList();
	private ArrayList friendsBlockList = new ArrayList();
	private ArrayList friendsForwardList = new ArrayList();

	private bool autoAddFriendsToAllowList = false;

	private string passportUrl = string.Empty;


	public string PassportURL
	{
		get { return this.passportUrl; }
		set { this.passportUrl = value; }
	}


	public bool AutoAddFriendsToAllowList
	{
		get { return autoAddFriendsToAllowList; }
		set { autoAddFriendsToAllowList = value; }
	}


	public int ContactListVersion
	{
		get { return this.contactListVersion; }
		set { this.contactListVersion = value; }
	}


	public ArrayList AllowList
	{
		get { return this.friendsAllowList; }
	}


	public ArrayList ForwardList
	{
		get { return this.friendsForwardList; }
	}


	public ArrayList ReverseList
	{
		get { return this.friendsReverseList; }
	}


	public ArrayList BlockList
	{
		get { return this.friendsBlockList; }
	}



	public Settings(IConstants constants, IStorage storage, ArrayList optionsList)
		: base(constants, storage, optionsList)
	{
		this.storage = storage;
	}


	public override void Load()
	{
		base.Load();

		this.passportUrl = this.storage.ReadString("PassportURL", string.Empty);

		// read friends lists from file
		string fileName = NBM.Config.Constants.UserDataPath +
			System.IO.Path.DirectorySeparatorChar + "msn_" + this.Username + ".bin";

		IO.FileStream fs = null;

		try
		{
			fs = new IO.FileStream(fileName, IO.FileMode.Open);
			BinaryFormatter bf = new BinaryFormatter();

			this.contactListVersion = (int)bf.Deserialize(fs);
			this.autoAddFriendsToAllowList = (bool)bf.Deserialize(fs);

			this.friendsAllowList = (ArrayList)bf.Deserialize(fs);
			this.friendsBlockList = (ArrayList)bf.Deserialize(fs);
			this.friendsForwardList = (ArrayList)bf.Deserialize(fs);
			this.friendsReverseList = (ArrayList)bf.Deserialize(fs);
		}
		catch
		{
			// problem loading - set the contact list version to zero to force
			// re-synchronization
			this.contactListVersion = 0;
			this.autoAddFriendsToAllowList = false;

			this.friendsAllowList.Clear();
			this.friendsBlockList.Clear();
			this.friendsForwardList.Clear();
			this.friendsReverseList.Clear();
		}
		finally
		{
			if (fs != null)
				fs.Close();
		}
	}


	public override void Save()
	{
		base.Save();

		this.storage.Write("PassportURL", this.passportUrl);

		// read friends lists from files
		string fileName = NBM.Config.Constants.UserDataPath +
			System.IO.Path.DirectorySeparatorChar + "msn_" + this.Username + ".bin";

		IO.FileStream fs = null;

		try
		{
			fs = new IO.FileStream(fileName, IO.FileMode.Create);
			BinaryFormatter bf = new BinaryFormatter();

			bf.Serialize(fs, this.contactListVersion);
			bf.Serialize(fs, this.autoAddFriendsToAllowList);

			bf.Serialize(fs, this.friendsAllowList);
			bf.Serialize(fs, this.friendsBlockList);
			bf.Serialize(fs, this.friendsForwardList);
			bf.Serialize(fs, this.friendsReverseList);
		}
		catch
		{
		}
		finally
		{
			if (fs != null)
				fs.Close();
		}
	}
}
