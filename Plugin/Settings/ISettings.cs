using System;

// 1/6/03

namespace NBM.Plugin
{
	/// <summary>
	/// All protocol's settings implement this interface.
	/// </summary>
	public interface ISettings
	{
		/// <summary>
		/// Loads settings
		/// </summary>
		void Load();


		/// <summary>
		/// Saves settings
		/// </summary>
		void Save();
	}
}
