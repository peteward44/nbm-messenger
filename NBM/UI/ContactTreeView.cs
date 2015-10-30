using System;
using System.Collections;
using NBM.Plugin;

using WinForms = System.Windows.Forms;
using Drawing = System.Drawing;

// 9/6/03

namespace NBM
{
	/// <summary>
	/// Contact tree view.
	/// </summary>
	public class ContactTreeView : TreeViewEx
	{
		private MainForm mainForm;
		private readonly Drawing.Color imageTransparentColour = Drawing.Color.FromArgb(255, 255, 255);
		private OfflineTreeNode offlineNode;
		private Hashtable protocolNodeTable = new Hashtable();


		/// <summary>
		/// Constructs a contact tree view.
		/// </summary>
		/// <param name="mainForm"></param>
		public ContactTreeView(MainForm mainForm)
		{
			this.mainForm = mainForm;

			this.ImageList.Images.Add(Drawing.Image.FromFile(Config.Constants.ImagePath + @"\online.bmp"), imageTransparentColour);
			this.ImageList.Images.Add(Drawing.Image.FromFile(Config.Constants.ImagePath + @"\away.bmp"), imageTransparentColour);
			this.ImageList.Images.Add(Drawing.Image.FromFile(Config.Constants.ImagePath + @"\busy.bmp"), imageTransparentColour);
			this.ImageList.Images.Add(Drawing.Image.FromFile(Config.Constants.ImagePath + @"\offline.bmp"), imageTransparentColour);
		}


		/// <summary>
		/// Hide the list of offline contacts.
		/// </summary>
		public void HideOfflineList()
		{
			if (this.Nodes.Contains(this.offlineNode))
				this.offlineNode.Remove();
		}


		/// <summary>
		/// Shows the list of offline contacts.
		/// </summary>
		public void ShowOfflineList()
		{
			if (!this.Nodes.Contains(this.offlineNode))
				this.Nodes.Add(this.offlineNode);
		}


		/// <summary>
		/// Adds the protocols to the contact list.
		/// </summary>
		/// <param name="protocols"></param>
		public void AddProtocols(ArrayList protocols)
		{
			this.offlineNode = new OfflineTreeNode();

			int index = 0;

			foreach (Protocol protocol in protocols)
			{
				string rootPath = Config.Constants.ImagePath + @"\" + protocol.Name.ToLower();

				// load images
				this.ImageList.Images.Add(Drawing.Image.FromFile(rootPath + @"\open\online.bmp"), imageTransparentColour);
				this.ImageList.Images.Add(Drawing.Image.FromFile(rootPath + @"\open\away.bmp"), imageTransparentColour);
				this.ImageList.Images.Add(Drawing.Image.FromFile(rootPath + @"\open\busy.bmp"), imageTransparentColour);
				this.ImageList.Images.Add(Drawing.Image.FromFile(rootPath + @"\open\offline.bmp"), imageTransparentColour);
				this.ImageList.Images.Add(Drawing.Image.FromFile(rootPath + @"\closed\online.bmp"), imageTransparentColour);
				this.ImageList.Images.Add(Drawing.Image.FromFile(rootPath + @"\closed\away.bmp"), imageTransparentColour);
				this.ImageList.Images.Add(Drawing.Image.FromFile(rootPath + @"\closed\busy.bmp"), imageTransparentColour);
				this.ImageList.Images.Add(Drawing.Image.FromFile(rootPath + @"\closed\offline.bmp"), imageTransparentColour);
				this.ImageList.Images.Add(Drawing.Image.FromFile(rootPath + @"\offline.bmp"), imageTransparentColour);

				// add node to tree
				ProtocolTreeNode protocolNode = new ProtocolTreeNode(protocol, this.offlineNode, index);
				protocolNode.Visible = protocol.Settings.Enabled;
				protocolNode.ContextMenu = new ProtocolMenu(this.mainForm, protocol);
				this.Nodes.Add(protocolNode);

				protocolNodeTable.Add(protocol, protocolNode);

				index++;
			}

			this.Nodes.Add(this.offlineNode);

			this.offlineNode.Collapse();
		}


		/// <summary>
		/// Returns the protocol node assocaited with the protocol passed in
		/// </summary>
		/// <param name="protocol"></param>
		/// <returns></returns>
		public ProtocolTreeNode GetProtocolNode(Protocol protocol)
		{
			return (ProtocolTreeNode)this.protocolNodeTable[protocol];
		}
	}
}
