using System;
using System.Collections;

// 14/6/03

namespace NBM.Plugin
{
	#region Protocol listener collection and enumerator


	/// <summary>
	/// 
	/// </summary>
	internal class ProtocolListenerEnumerator : IEnumerator, IDisposable
	{
		private int index = -1;
		private IList collection;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		public ProtocolListenerEnumerator(ProtocolListenerCollection collection)
		{
			this.collection = ArrayList.Synchronized(collection.Clone());
		}


		/// <summary>
		/// 
		/// </summary>
		object IEnumerator.Current
		{
			get { return Current; }
		}


		/// <summary>
		/// 
		/// </summary>
		public IProtocolListener Current
		{
			get { return (IProtocolListener)collection[index]; }
		}


		/// <summary>
		/// 
		/// </summary>
		public void Reset()
		{
			index = -1;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public bool MoveNext()
		{
			if (index == -1)
				System.Threading.Monitor.Enter(collection.SyncRoot);

			return ++index < collection.Count;
		}


		/// <summary>
		/// 
		/// </summary>
		public void Dispose()
		{
			System.Threading.Monitor.Exit(collection.SyncRoot);
		}
	}


	/// <summary>
	/// 
	/// </summary>
	internal class ProtocolListenerCollection : IList, ICollection, IEnumerable, ICloneable
	{
		private ArrayList arrayList = new ArrayList();


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		object ICloneable.Clone()
		{
			return this.Clone();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public ProtocolListenerCollection Clone()
		{
			ProtocolListenerCollection c = new ProtocolListenerCollection();
			c.arrayList.AddRange(this.arrayList);
			return c;
		}


		/// <summary>
		/// 
		/// </summary>
		public ProtocolListenerCollection()
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return new ProtocolListenerEnumerator(this);
		}


		/// <summary>
		/// 
		/// </summary>
		public int Count
		{
			get { return this.arrayList.Count; }
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="count"></param>
		public void CopyTo(Array array, int count)
		{
			this.arrayList.CopyTo(array, count);
		}


		/// <summary>
		/// 
		/// </summary>
		public bool IsSynchronized
		{
			get { return this.arrayList.IsSynchronized; }
		}


		/// <summary>
		/// 
		/// </summary>
		public object SyncRoot
		{
			get { return this.arrayList.SyncRoot; }
		}


		/// <summary>
		/// 
		/// </summary>
		public bool IsFixedSize
		{
			get { return arrayList.IsFixedSize; }
		}


		/// <summary>
		/// 
		/// </summary>
		public bool IsReadOnly
		{
			get { return arrayList.IsReadOnly; }
		}


		/// <summary>
		/// 
		/// </summary>
		object IList.this[int index]
		{
			get { return this[index]; }
			set { this[index] = (IProtocolListener)value; }
		}


		/// <summary>
		/// 
		/// </summary>
		public IProtocolListener this[int index]
		{
			get { return (IProtocolListener)this.arrayList[index]; }
			set
			{
				this.arrayList[index] = value;
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		int IList.Add(object o)
		{
			return Add((IProtocolListener)o);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public int Add(IProtocolListener o)
		{
			return this.arrayList.Add(o);
		}


		/// <summary>
		/// 
		/// </summary>
		public void Clear()
		{
			arrayList.Clear();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		bool IList.Contains(object o)
		{
			return Contains((IProtocolListener)o);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public bool Contains(IProtocolListener o)
		{
			return this.arrayList.Contains(o);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		int IList.IndexOf(object o)
		{
			return IndexOf((IProtocolListener)o);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public int IndexOf(IProtocolListener o)
		{
			return this.arrayList.IndexOf(o);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="o"></param>
		void IList.Insert(int index, object o)
		{
			Insert(index, (IProtocolListener)o);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="o"></param>
		public void Insert(int index, IProtocolListener o)
		{
			this.arrayList.Insert(index, o);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		void IList.Remove(object o)
		{
			Remove((IProtocolListener)o);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		public void Remove(IProtocolListener o)
		{
			this.arrayList.Remove(o);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			arrayList.RemoveAt(index);
		}
	}


	#endregion


	/// <summary>
	/// Server class for ProtocolControl.
	/// <seealso cref="ProtocolControl"/>
	/// </summary>
	public class ProtocolServer
	{
		private ProtocolListenerCollection listenerList = new ProtocolListenerCollection();
		private ArrayList friendsList = new ArrayList();


		/// <summary>
		/// 
		/// </summary>
		public ProtocolServer()
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="listener"></param>
		public void AddListener(IProtocolListener listener)
		{
			if (!this.listenerList.Contains(listener))
				listenerList.Add(listener);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="listener"></param>
		public void RemoveListener(IProtocolListener listener)
		{
			listenerList.Remove(listener);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public Proxy.IConnection CreateConnection()
		{
			switch (Settings.GlobalSettings.Instance().InternetConnection)
			{
				default:
				case Settings.InternetConnection.Direct:
					return new Proxy.DirectConnection();

				case Settings.InternetConnection.Socks4:
					return new Proxy.Socks4Connection();

				case Settings.InternetConnection.Socks5:
					return new Proxy.Socks5Connection();

				case Settings.InternetConnection.Http:
					return new Proxy.HttpConnection();
			}
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnBeginConnect()
		{
			foreach (IProtocolListener listener in this.listenerList)
			{
				listener.OnBeginConnect();
			}
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnConnect()
		{
			foreach (IProtocolListener listener in this.listenerList)
			{
				listener.OnConnect();
			}
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnConnectCanceled()
		{
			foreach (IProtocolListener listener in this.listenerList)
			{
				listener.OnConnectCanceled();
			}
		}


		/// <summary>
		/// 
		/// </summary>
		public void OnNormalDisconnect()
		{
			this.friendsList.Clear();

			foreach (IProtocolListener listener in this.listenerList)
			{
				listener.OnDisconnect(false);
			}
		}


		/// <summary>
		/// 
		/// </summary>
		public void ForcedDisconnect()
		{
			this.friendsList.Clear();

			foreach (IProtocolListener listener in this.listenerList)
			{
				listener.OnDisconnect(true);
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="newStatus"></param>
		public void OnChangeStatus(OnlineStatus newStatus)
		{
			foreach (IProtocolListener listener in this.listenerList)
			{
				listener.OnChangeStatus(newStatus);
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="opCompleteEvent"></param>
		/// <param name="tag"></param>
		public void StartInvitedConversation(Friend friend, OperationCompleteEvent opCompleteEvent, object tag)
		{
			foreach (IProtocolListener listener in this.listenerList)
			{
				listener.OnInvitedToConversation(friend, opCompleteEvent, tag);
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void AddFriend(Friend friend)
		{
			foreach (IProtocolListener listener in this.listenerList)
			{
				friend.AddListener(listener);
				listener.OnFriendAdd(friend);
			}

			this.friendsList.Add(friend);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void RemoveFriend(Friend friend)
		{
			foreach (IProtocolListener listener in this.listenerList)
			{
				friend.RemoveListener(listener);
				listener.OnFriendRemove(friend);
			}

			this.friendsList.Remove(friend);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"></param>
		public void RemoveFriend(string username)
		{
			Friend friend = GetFriend(username);
			if (friend != null)
				RemoveFriend(friend);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <returns></returns>
		public bool ContainsFriend(Friend friend)
		{
			return this.friendsList.Contains(friend);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public bool ContainsFriend(string username)
		{
			return GetFriend(username) != null;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public Friend GetFriend(string username)
		{
			lock (this.friendsList.SyncRoot)
			{
				foreach (Friend friend in this.friendsList)
				{
					if (friend.Username == username)
						return friend;
				}
			}

			return null;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"></param>
		public void AddFriendToList(string username)
		{
			foreach (IProtocolListener listener in this.listenerList)
			{
				listener.OnAddFriendToList(username);
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void RemoveFriendFromList(Friend friend)
		{
			foreach (IProtocolListener listener in this.listenerList)
			{
				listener.OnRemoveFriendFromList(friend);
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void BlockFriend(Friend friend)
		{
			foreach (IProtocolListener listener in this.listenerList)
			{
				listener.OnBlockFriend(friend);
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		public void UnblockFriend(Friend friend)
		{
			foreach (IProtocolListener listener in this.listenerList)
			{
				listener.OnUnblockFriend(friend);
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		public void WriteDebug(string text)
		{
			foreach (IProtocolListener listener in this.listenerList)
			{
				listener.OnWriteDebug(text);
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		public void WriteDebug(string format, params object[] args)
		{
			foreach (IProtocolListener listener in this.listenerList)
			{
				listener.OnWriteDebug(string.Format(format, args));
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="friend"></param>
		/// <param name="reason"></param>
		/// <param name="enableAddCheckbox"></param>
		public void PromptForStrangerHasAddedMe(Friend friend, string reason, bool enableAddCheckbox)
		{
			foreach (IProtocolListener listener in this.listenerList)
			{
				listener.OnPromptForStrangerHasAddedMe(friend, reason, enableAddCheckbox);
			}
		}
	}
}
