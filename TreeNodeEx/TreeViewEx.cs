using System;
using System.Collections;
using Drawing = System.Drawing;
using Drawing2D = System.Drawing.Drawing2D;
using WinForms = System.Windows.Forms;

using System.Runtime.InteropServices;

// 4/6/03

public class TreeViewExEventArgs
{
	private TreeNodeEx node;

	public TreeNodeEx Node
	{
		get { return node; }
	}


	public TreeViewExEventArgs(TreeNodeEx node)
	{
		this.node = node;
	}
}


public delegate void TreeViewExEventHandler(object sender, TreeViewExEventArgs args);


/// <summary>
/// Contact tree view.
/// </summary>
public class TreeViewEx : WinForms.Control
{
	private TreeNodeCollectionEx nodes;
	private TreeNodeEx mouseOverNode = null, selectedNode = null, dragOverNode = null;
	private int widthIndent = 5;
	private int heightIndent = 2;
	private WinForms.ImageList imageList = new WinForms.ImageList();
	private WinForms.VScrollBar vScrollBar;
	private Drawing2D.Matrix viewMatrix = new Drawing2D.Matrix();
	private WinForms.ToolTip toolTip;
	private int updateCount = 0;


	[DllImport("user32.dll")]
	private static extern void SendMessage(IntPtr hwnd, int msg, int wparam, int lparam);


	#region Public fields


	public Drawing.Brush TextBrush = Drawing.Brushes.Black;
	public Drawing.Brush TextMouseOverBrush = Drawing.Brushes.DarkBlue;
	public Drawing.Brush TextSelectedBrush = Drawing.Brushes.Red;

	public bool ChangeColorOnMouseOver = true;
	public bool ChangeColorOnSelected = false;
	public bool AllowMovementOfNodes = false;

	public bool ShowPlusMinus = false;


	#endregion


	public event TreeViewExEventHandler Expand, Collapse, NodeMove;


	internal void ExecuteExpand(TreeNodeEx node)
	{
		if (this.Expand != null)
			this.Expand(this, new TreeViewExEventArgs(node));
	}


	internal void ExecuteCollapse(TreeNodeEx node)
	{
		if (this.Collapse != null)
			this.Collapse(this, new TreeViewExEventArgs(node));

		if (!this.vScrollBar.Visible)
		{
			this.Invalidate();
			this.Update();
		}
	}


	#region Properties


	public int WidthIndent
	{
		get { return widthIndent; }
		set { widthIndent = value; }
	}


	public int HeightIndent
	{
		get { return heightIndent; }
		set { heightIndent = value; }
	}


	public WinForms.ImageList ImageList
	{
		get { return imageList; }
	}


	public TreeNodeCollectionEx Nodes
	{
		get { return nodes; }
	}


	#endregion


	public TreeViewEx()
	{
		this.nodes = new TreeNodeCollectionEx(this);

		// initialise scroll bar
		this.vScrollBar = new WinForms.VScrollBar();

		this.vScrollBar.Minimum = this.vScrollBar.Maximum = 0;
		this.vScrollBar.Dock = WinForms.DockStyle.Right;
		this.vScrollBar.Visible = false;

		this.vScrollBar.Scroll += new WinForms.ScrollEventHandler(OnVerticalScroll);

		this.Controls.Add( vScrollBar );

		// initialise tool tip
		this.toolTip = new WinForms.ToolTip();
		this.toolTip.Active = true;

		this.AllowDrop = this.AllowMovementOfNodes;

		this.SetStyle(WinForms.ControlStyles.DoubleBuffer | WinForms.ControlStyles.UserPaint |
WinForms.ControlStyles.AllPaintingInWmPaint, true);

		this.BackColor = Drawing.Color.White;
	}


	protected override void OnResize(EventArgs e)
	{
		this.Invalidate();
		this.Update();
		base.OnResize(e);
	}


	#region Scrolling methods


	private void OnVerticalScroll(object sender, WinForms.ScrollEventArgs args)
	{
		this.viewMatrix.Reset();
		this.viewMatrix.Translate(0.0f, -args.NewValue);
		this.Invalidate();
		this.Update();
	}


	#endregion


	#region Node methods


	public void ExpandAll()
	{
		foreach (TreeNodeEx node in this.Nodes)
		{
			node.Expand();
			ExpandChildren(node);
		}
	}


	private void ExpandChildren(TreeNodeEx node)
	{
		foreach (TreeNodeEx childNode in node.Nodes)
		{
			childNode.Expand();
			ExpandChildren(childNode);
		}
	}


	public void CollapseAll()
	{
		foreach (TreeNodeEx node in this.Nodes)
		{
			node.Collapse();
			CollapseChildren(node);
		}
	}


	private void CollapseChildren(TreeNodeEx node)
	{
		foreach (TreeNodeEx childNode in node.Nodes)
		{
			childNode.Collapse();
			CollapseChildren(childNode);
		}
	}


	#endregion


	#region Drag / drop methods


	protected override void OnDragDrop(WinForms.DragEventArgs args)
	{
		if (AllowMovementOfNodes)
		{
			TreeNodeEx node = (TreeNodeEx)args.Data.GetData(typeof(TreeNodeEx));

			if (node != dragOverNode)
			{
				// drop the node into the node its over
				if (this.dragOverNode.Parent != null)
				{
					node.Remove();
					this.dragOverNode.Parent.Nodes.Insert(this.dragOverNode.Parent.Nodes.IndexOf(dragOverNode), node);
				}
				else
				{
					node.Remove();
					this.Nodes.Insert(this.Nodes.IndexOf(dragOverNode), node);
				}

				if (this.NodeMove != null)
					this.NodeMove(this, new TreeViewExEventArgs(node));

				this.Invalidate();
				this.Update();
			}

			this.dragOverNode = null;
		}

		base.OnDragDrop(args);
	}


	protected override void OnDragEnter(WinForms.DragEventArgs args)
	{
		if (AllowMovementOfNodes)
		{
			args.Effect = (args.Data.GetDataPresent(typeof(TreeNodeEx))) ?
				WinForms.DragDropEffects.Link : WinForms.DragDropEffects.None;
		}

		base.OnDragEnter(args);
	}


	protected override void OnDragLeave(EventArgs args)
	{
		if (AllowMovementOfNodes)
		{
			this.dragOverNode = null;
		}

		base.OnDragLeave(args);
	}


	protected override void OnDragOver(WinForms.DragEventArgs args)
	{
		if (AllowMovementOfNodes)
		{
			TreeNodeEx draggedNode = (TreeNodeEx)args.Data.GetData(typeof(TreeNodeEx));

			// get node which the mouse is over
			if (dragOverNode != null)
			{
				this.Invalidate( new Drawing.Rectangle(
					dragOverNode.Location.X,
					dragOverNode.Location.Y - this.HeightIndent,
					this.Width - dragOverNode.Location.X,
					dragOverNode.Size.Height + this.HeightIndent)
					);
			}

			TreeNodeEx draggedOverNode = (TreeNodeEx)this.GetNodeAt( this.PointToClient( WinForms.Control.MousePosition ) );

			if (draggedNode.IsParent(draggedOverNode))
			{
				this.dragOverNode = null;
				args.Effect = WinForms.DragDropEffects.None;
			}
			else
			{
				this.dragOverNode = draggedOverNode;

				if (dragOverNode != null)
				{
					this.Invalidate( new Drawing.Rectangle(
						dragOverNode.Location.X,
						dragOverNode.Location.Y - this.HeightIndent,
						this.Width - dragOverNode.Location.X,
						dragOverNode.Size.Height + this.HeightIndent)
						);
				}

				args.Effect = WinForms.DragDropEffects.Link;
			}

			this.Update();
		}

		base.OnDragOver(args);
	}


	#endregion


	#region Mouse movement methods


	protected override void OnMouseMove(WinForms.MouseEventArgs args)
	{
		TreeNodeEx node = this.GetNodeAt(new Drawing.Point(args.X, args.Y));

		if (this.ChangeColorOnMouseOver)
		{
			if (mouseOverNode != null)
				mouseOverNode.Invalidate();

			if (node != null && node != mouseOverNode)
				node.Invalidate();
			else
				mouseOverNode = null;

			mouseOverNode = node;

			this.Update();
		}

		if (node != null)
			this.toolTip.SetToolTip(this, node.ToolTipText);

		base.OnMouseMove(args);
	}


	protected override void OnMouseDown(WinForms.MouseEventArgs args)
	{
		TreeNodeEx node = GetNodeAt(new Drawing.Point(args.X, args.Y));

		switch (args.Button)
		{
			case WinForms.MouseButtons.Left:
				if (
					(node != null	// clicked on image without showplusminus
					&& !this.imageList.Images.Empty && !this.ShowPlusMinus
					&& args.X - node.Location.X < this.imageList.ImageSize.Width)
					|| // or clicked on the plus/minus
					(node != null
					&& this.ShowPlusMinus
					&& args.X - node.Location.X < 15))
				{
					node.Toggle();
				}
				else
				{
					// clicked on text
					if (this.selectedNode != null)
						this.selectedNode.Invalidate();

					this.selectedNode = node;

					// then redraw area
					if (ChangeColorOnSelected && this.selectedNode != null)
					{
						selectedNode.Invalidate();
						this.selectedNode.ExecuteSelected(new EventArgs());
						this.Update();
					}
				}

				if (node != null && this.AllowMovementOfNodes)
					this.DoDragDrop(node, WinForms.DragDropEffects.Link);

				break;

			case WinForms.MouseButtons.Right:
				// right button, show context menu
				if (node.ContextMenu != null)
				{
					node.ContextMenu.Show(this, new Drawing.Point(args.X, args.Y));
				}
				break;
		}

		base.OnMouseDown(args);
	}


	protected override void OnMouseLeave(EventArgs args)
	{
		this.toolTip.SetToolTip(this, string.Empty);
		if (this.mouseOverNode != null)
		{
			this.mouseOverNode.Invalidate();
			this.mouseOverNode = null;
			this.Update();
		}

		base.OnMouseLeave(args);
	}


	#endregion


	#region Rendering methods


	private delegate void UpdateHandler();


	public void BeginUpdate()
	{
		if (updateCount++ == 0)
			this.Invoke(new UpdateHandler(pBeginUpdate));
	}


	public void EndUpdate()
	{
		if (updateCount > 0)
		{
			if (--updateCount == 0)
				this.Invoke(new UpdateHandler(pEndUpdate));
		}
	}


	private void pBeginUpdate()
	{
		SendMessage(this.Handle, 11 /* WM_SETREDRAW */, 0, 0);
	}


	private void pEndUpdate()
	{
		SendMessage(this.Handle, 11 /* WM_SETREDRAW */, -1, 0);
		this.Invalidate();
		this.Update();
	}


	protected override void OnPaint(WinForms.PaintEventArgs args)
	{
		Drawing.Graphics graphics = args.Graphics;

		graphics.Clear(this.BackColor);
		this.DrawNodes(graphics);

		base.OnPaint(args);
	}


	private void DrawNodes(Drawing.Graphics graphics)
	{
		Drawing2D.Matrix matrix = viewMatrix.Clone();
		int totalHeight = 0;

		foreach (TreeNodeEx treeNode in this.nodes)
		{
			DrawNode(treeNode, graphics, matrix, ref totalHeight);
			matrix.Translate(0.0f, this.HeightIndent + this.Font.Height);
		}

		this.vScrollBar.Visible = (totalHeight > this.Height);

		if (this.vScrollBar.Visible)
			this.vScrollBar.Maximum = totalHeight - this.Height + this.HeightIndent + this.Font.Height;
		else
		{
			this.viewMatrix.Reset();
			this.vScrollBar.Value = 0;
		}
	}


	private void DrawNode(TreeNodeEx node, Drawing.Graphics graphics, Drawing2D.Matrix matrix, ref int totalHeight)
	{
		if (node.Visible)
		{
			totalHeight += this.HeightIndent + this.Font.Height;

			// transform coordinates
			Drawing.PointF[] p = new Drawing.PointF[1];
			p[0] = new Drawing.PointF(0.0f, 0.0f);
			matrix.TransformPoints(p);

			// calculate location and size
			node.Location = new Drawing.Point((int)p[0].X, (int)p[0].Y);
			node.Size = new Drawing.Size(this.Width, this.Font.Height);

			if (dragOverNode == node)
			{
				// draw drag over line
				graphics.DrawLine(Drawing.Pens.Black, node.Location.X, node.Location.Y, this.Width, node.Location.Y);
			}

			// draw plus/minus
			if (this.ShowPlusMinus)
			{
				if (node.Nodes.Count > 0)
				{
					graphics.DrawRectangle(Drawing.Pens.Black, (int)p[0].X + 2, (int)p[0].Y + 2, 8, 8);

					if (!node.Expanded)
					{
						// line across
						graphics.DrawLine(Drawing.Pens.Black, (int)p[0].X + 4, (int)p[0].Y + 6, (int)p[0].X + 8, (int)p[0].Y + 6);
						// down
						graphics.DrawLine(Drawing.Pens.Black, (int)p[0].X + 6, (int)p[0].Y + 4, (int)p[0].X + 6, (int)p[0].Y + 8);
					}
					else
					{
						graphics.DrawLine(Drawing.Pens.Black, (int)p[0].X + 4, (int)p[0].Y + 6, (int)p[0].X + 8, (int)p[0].Y + 6);
					}
				}

				p[0].X += 12;
			}

			// draw node image
			if (!this.imageList.Images.Empty)
			{
				this.imageList.Draw(graphics, new Drawing.Point((int)p[0].X, (int)p[0].Y), (node.Expanded) ? node.ExpandedImageIndex : node.CollapsedImageIndex);
				p[0].X += this.imageList.ImageSize.Width;
			}

			// draw node text
			Drawing.Brush textBrush;

			if (node == this.selectedNode && this.ChangeColorOnSelected)
				textBrush = this.TextSelectedBrush;
			else if (node == this.mouseOverNode && this.ChangeColorOnMouseOver)
				textBrush = this.TextMouseOverBrush;
			else
				textBrush = node.TextBrush;

			Drawing.Font font = new Drawing.Font(this.Font.FontFamily.Name, this.Font.Size, node.FontStyle);
			graphics.DrawString(node.Text, font, textBrush, p[0]);

			// go through children
			if (node.Expanded && node.Nodes.Count > 0)
			{
				matrix.Translate((float)this.WidthIndent, 0.0f);

				foreach (TreeNodeEx treeNode in node.Nodes)
				{
					matrix.Translate(0.0f, this.HeightIndent + this.Font.Height);
					DrawNode(treeNode, graphics, matrix, ref totalHeight);
				}

				matrix.Translate(-(float)this.WidthIndent, 0.0f);
			}
		}
		else
			matrix.Translate(0.0f, -((float)this.HeightIndent + this.Font.Height));
	}


	#endregion


	#region Node position determination methods


	public TreeNodeEx GetNodeAt(Drawing.Point p)
	{
		int childHeight = (int)this.viewMatrix.OffsetY;
		return FindChildAtPoint(this.nodes, ref childHeight, p);
	}


	private TreeNodeEx FindChildAtPoint(TreeNodeCollectionEx nodes, ref int currentPosition, Drawing.Point p)
	{
		foreach (TreeNodeEx node in nodes)
		{
			if (node.Visible)
			{
				currentPosition += this.HeightIndent + this.Font.Height;

				if (currentPosition > p.Y)
					return node;

				if (node.Expanded)
				{
					TreeNodeEx node2 = FindChildAtPoint(node.Nodes, ref currentPosition, p);
					if (node2 != null)
						return node2;
				}
			}
		}

		return null;
	}


	#endregion


}
