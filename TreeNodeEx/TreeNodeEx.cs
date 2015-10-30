using System;
using System.Collections;

using Drawing = System.Drawing;
using Drawing2D = System.Drawing.Drawing2D;
using WinForms = System.Windows.Forms;

// 4/6/03

/// <summary>
/// Summary description for ContactTreeNode.
/// </summary>
public class TreeNodeEx : ICloneable
{
	private TreeNodeCollectionEx nodes;
	private string text = string.Empty, toolTipText = string.Empty;
	private bool isExpanded = true;
	private Drawing.Point location;
	private Drawing.Size size = new Drawing.Size(200, 50);
	private TreeViewEx treeView = null;
	private TreeNodeEx parent = null;
	private object tag;
	private WinForms.ContextMenu contextMenu = null;
	private bool visible = true;

	private int collapsedImageIndex = 0, expandedImageIndex = 0;

	public event EventHandler Selected;


	private Drawing.FontStyle fontStyle = Drawing.FontStyle.Regular;
	private Drawing.Brush textBrush = Drawing.Brushes.Black;


	public Drawing.Brush TextBrush
	{
		get { return textBrush; }
		set
		{
			textBrush = value;
			this.Invalidate();
			this.Update();
		}
	}


	public bool Visible
	{
		get { return this.visible; }
		set
		{
			this.visible = value;
			
			if (this.treeView != null)
				this.treeView.Refresh();
		}
	}


	public Drawing.FontStyle FontStyle
	{
		get { return fontStyle; }
		set
		{
			fontStyle = value;
			this.Invalidate();
			this.Update();
		}
	}


	public object Tag
	{
		get { return tag; }
		set { tag = value; }
	}


	public string ToolTipText
	{
		get { return toolTipText; }
		set { toolTipText = value; }
	}


	public WinForms.ContextMenu ContextMenu
	{
		get { return this.contextMenu; }
		set { this.contextMenu = value; }
	}


	public int CollapsedImageIndex
	{
		get { return collapsedImageIndex; }
		set
		{
			if (value < 0)
				throw new IndexOutOfRangeException("Image index must be greated than zero");

			if (value != this.collapsedImageIndex && this.treeView != null)
			{
				this.Invalidate();
				this.Update();
			}

			this.collapsedImageIndex = value;
		}
	}


	public int ExpandedImageIndex
	{
		get { return this.expandedImageIndex; }
		set
		{
			if (value < 0)
				throw new IndexOutOfRangeException("Image index must be greated than zero");

			if (value != this.expandedImageIndex && this.treeView != null)
			{
				this.Invalidate();
				this.Update();
			}

			this.expandedImageIndex = value;
		}
	}


	internal void ExecuteSelected(EventArgs e)
	{
		if (Selected != null)
			Selected(this, e);
	}


	public TreeViewEx TreeView
	{
		get { return treeView; }
	}


	internal TreeViewEx iTreeView
	{
		get { return treeView; }
		set
		{
			treeView = value;
			foreach (TreeNodeEx node in this.Nodes)
			{
				node.iTreeView = value;
			}
		}
	}


	public TreeNodeEx Parent
	{
		get { return parent; }
	}


	internal TreeNodeEx iParent
	{
		get { return parent; }
		set { parent = value; }
	}


	public Drawing.Point Location
	{
		get { return location; }
		set { location = value; }
	}


	public Drawing.Size Size
	{
		get { return size; }
		set { size = value; }
	}


	public Drawing.Rectangle Rectangle
	{
		get { return new Drawing.Rectangle(location, size); }
	}


	public void Invalidate()
	{
		if (this.treeView != null)
		{
			Drawing.Rectangle rect = this.Rectangle;
			rect.X -= 4;
			rect.Y -= 4;
			rect.Width += 8;
			rect.Height += 8;
			this.treeView.Invalidate(rect);
		}
	}


	public TreeNodeCollectionEx Nodes
	{
		get { return nodes; }
	}


	public string Text
	{
		get { return text; }
		set { text = value; }
	}


	public bool Expanded
	{
		get { return isExpanded; }
	}


	public bool Collapsed
	{
		get { return !isExpanded; }
	}


	public void Toggle()
	{
		if (!this.isExpanded)
			this.Expand();
		else
			this.Collapse();
	}


	public void Expand()
	{
		this.isExpanded = true;
		if (this.treeView != null)
		{
			this.treeView.Invalidate();
			this.treeView.Update();

			this.treeView.ExecuteExpand(this);
		}
	}


	public void Collapse()
	{
		this.isExpanded = false;
		if (this.treeView != null)
		{
			this.treeView.Invalidate();
			this.treeView.Update();

			this.treeView.ExecuteCollapse(this);
		}
	}


	public void Remove()
	{
		if (this.parent != null)
			this.parent.Nodes.Remove(this);
		else if (this.treeView != null)
			this.treeView.Nodes.Remove(this);

		if (this.treeView != null)
		{
			this.treeView.Invalidate();
			this.treeView.Update();
		}
	}



	public void Update()
	{
		if (this.treeView != null)
			this.treeView.Update();
	}

	/// <summary>
	/// Checks if this node is either a direct or indirect parent of
	/// the parameter node.
	/// </summary>
	/// <param name="node">Child node to check</param>
	/// <returns>True if this node is a parent to the parameter node</returns>
	public bool IsParent(TreeNodeEx node)
	{
		if (node == this)
			return true;

		foreach (TreeNodeEx child in this.Nodes)
		{
			if (child.IsParent(node))
				return true;
		}

		return false;
	}


	public TreeNodeEx()
	{
		this.nodes = new TreeNodeCollectionEx(this);
	}


	public TreeNodeEx(string text)
		: this()
	{
		this.text = text;
	}


	public TreeNodeEx(string text, string tooltip)
		: this(text)
	{
		this.toolTipText = tooltip;
	}


	public object Clone()
	{
		return this.MemberwiseClone();
	}
}
