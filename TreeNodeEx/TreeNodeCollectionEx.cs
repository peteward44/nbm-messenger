using System;
using System.Collections;
using WinForms = System.Windows.Forms;

// 5/6/03


internal class TreeNodeCollectionExEnumerator : IEnumerator, IDisposable
{
	private int index = -1;
	private TreeNodeCollectionEx collection;


	public TreeNodeCollectionExEnumerator(TreeNodeCollectionEx collection)
	{
		this.collection = collection;
	}


	object IEnumerator.Current
	{
		get { return Current; }
	}


	public TreeNodeEx Current
	{
		get { return collection[index]; }
	}


	public void Reset()
	{
		index = -1;
	}


	public bool MoveNext()
	{
//		if (index == -1)
//			System.Threading.Monitor.Enter(collection);

		index++;
		return index < collection.Count;
	}


	public void Dispose()
	{
//		System.Threading.Monitor.Exit(collection);
	}
}


/// <summary>
/// Summary description for TreeNodeCollectionEx.
/// </summary>
public class TreeNodeCollectionEx : IList, ICollection, IEnumerable
{
	private ArrayList arrayList = new ArrayList();
	private TreeViewEx treeView;
	private TreeNodeEx node;


	public TreeNodeCollectionEx(TreeNodeEx node)
	{
		this.node = node;
	}


	public TreeNodeCollectionEx(TreeViewEx treeView)
	{
		this.treeView = treeView;
	}


	#region ICollection members


	public int Count
	{
		get { return arrayList.Count; }
	}


	public bool IsSynchronized
	{
		get { return arrayList.IsSynchronized; }
	}


	public object SyncRoot
	{
		get { return arrayList.SyncRoot; }
	}


	public void CopyTo(Array array, int index)
	{
		arrayList.CopyTo(array, index);
	}


	#endregion


	#region IEnumerable members


	public IEnumerator GetEnumerator()
	{
		return new TreeNodeCollectionExEnumerator(this);
	}


	#endregion


	#region IList members


	public bool IsFixedSize
	{
		get { return arrayList.IsFixedSize; }
	}


	public bool IsReadOnly
	{
		get { return arrayList.IsReadOnly; }
	}


	object IList.this[int index]
	{
		get { return this[index]; }
		set { this[index] = (TreeNodeEx)value; }
	}


	int IList.Add(object o)
	{
		return Add((TreeNodeEx)o);
	}


	public void Clear()
	{
		lock (this)
		{
			foreach (TreeNodeEx node in this.arrayList)
			{
				PreRemoveTreeNode(node);
			}

			arrayList.Clear();
		}
	}


	bool IList.Contains(object o)
	{
		return Contains((TreeNodeEx)o);
	}


	int IList.IndexOf(object o)
	{
		return IndexOf((TreeNodeEx)o);
	}


	void IList.Insert(int index, object o)
	{
		Insert(index, (TreeNodeEx)o);
	}


	void IList.Remove(object o)
	{
		Remove((TreeNodeEx)o);
	}


	#endregion


	public void RemoveAt(int index)
	{
		lock (this)
		{
			TreeNodeEx node = (TreeNodeEx)arrayList[index];
			PreRemoveTreeNode(node);
			arrayList.RemoveAt(index);
		}
	}


	private void PreAddTreeNode(TreeNodeEx o)
	{
		lock (this)
		{
			if (node != null)
			{
				o.iParent = node;
				o.iTreeView = node.TreeView;
			}
			else
			{
				o.iParent = null;
				o.iTreeView = this.treeView;
			}

			if (o.iTreeView != null)
			{
				o.iTreeView.Invalidate();
				o.iTreeView.Update();
			}
		}
	}


	public TreeNodeEx this[int index]
	{
		get { return (TreeNodeEx)arrayList[index]; }
		set
		{
			lock (this)
			{
				TreeNodeEx node = value;
				PreAddTreeNode(node);
				arrayList[index] = node;
			}
		}
	}


	private void PreRemoveTreeNode(TreeNodeEx o)
	{
		if (o.iTreeView != null)
		{
			o.iTreeView.Invalidate();
			o.iTreeView.Update();
		}
//		o.iTreeView = null;
	}


	public int Add(TreeNodeEx o)
	{
		lock (this)
		{
			PreAddTreeNode(o);
			return arrayList.Add(o);
		}
	}


	public bool Contains(TreeNodeEx o)
	{
		return arrayList.Contains(o);
	}


	public int IndexOf(TreeNodeEx o)
	{
		return arrayList.IndexOf(o);
	}


	public void Insert(int index, TreeNodeEx o)
	{
		lock (this)
		{
			PreAddTreeNode(o);
			arrayList.Insert(index, o);
		}
	}


	public void Remove(TreeNodeEx o)
	{
		lock (this)
		{
			PreRemoveTreeNode(o);
			arrayList.Remove(o);
		}
	}
}
