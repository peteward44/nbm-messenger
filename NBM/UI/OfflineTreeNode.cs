using System;
using System.Collections;
using WinForms = System.Windows.Forms;
using NBM.Plugin;

// 9/6/03

namespace NBM
{
	/// <summary>
	/// Treenode for offline contacts.
	/// </summary>
	public class OfflineTreeNode : TreeNodeEx
	{
		/// <summary>
		/// 
		/// </summary>
		public OfflineTreeNode()
			: base("Offline")
		{
			this.ExpandedImageIndex = this.CollapsedImageIndex = 3;
		}
	}
}
