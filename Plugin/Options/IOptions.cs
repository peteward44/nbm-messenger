using System;
using WinForms = System.Windows.Forms;

// 15/6/03

namespace NBM.Plugin
{
	/// <summary>
	///	Interface for options form.
	///	Plugins can implement this as many times as they like, one for
	///	each "section" on the options form.
	///	Each one of these must be added to the arraylist passed to ProtocolSetting's
	///	constructor.
	///	
	///	Every class which implements this interface must also derive from
	///	System.Windows.Forms.UserControl
	/// </summary>
	public interface IOptions : IDisposable
	{
		/// <summary>
		/// Name to show up on the option's form treeview.
		/// </summary>
		string NodeName
		{
			get;
		}


		/// <summary>
		/// Override to save form settings to the appropriate ProtocolSettings class.
		/// </summary>
		void Save();
	}
}
