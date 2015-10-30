using System;
using System.Collections;
using NBM.Plugin;

// 15/5/03

public class Settings : ProtocolSettings
{
	public Settings(IConstants constants, IStorage storage, ArrayList optionsList)
		: base(constants, storage, optionsList)
	{

	}


	public override void Load()
	{
		base.Load();
	}
}
