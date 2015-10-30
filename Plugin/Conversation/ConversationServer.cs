using System;
using System.Collections;

// 13/6/03

namespace NBM.Plugin
{
	#region Conversation enumerator and collection classes


	/// <summary>
	/// Enumerates over ConversationListenerCollection items
	/// </summary>
	internal class ConversationListenerEnumerator : IEnumerator, IDisposable
	{
		private int index = -1;
		private IList collection;


		/// <summary>
		/// 
		/// </summary>
		/// <param name="collection"></param>
		public ConversationListenerEnumerator(ConversationListenerCollection collection)
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
		public IConversationListener Current
		{
			get { return (IConversationListener)collection[index]; }
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
	/// Contains type-safe and thread-safe access to a ConversationListener collection.
	/// </summary>
	internal class ConversationListenerCollection : IList, ICollection, IEnumerable, ICloneable
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
		public ConversationListenerCollection Clone()
		{
			ConversationListenerCollection c = new ConversationListenerCollection();
			c.arrayList.AddRange(this.arrayList);
			return c;
		}


		/// <summary>
		/// 
		/// </summary>
		public ConversationListenerCollection()
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return new ConversationListenerEnumerator(this);
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
			set { this[index] = (IConversationListener)value; }
		}


		/// <summary>
		/// 
		/// </summary>
		public IConversationListener this[int index]
		{
			get { return (IConversationListener)this.arrayList[index]; }
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
			return Add((IConversationListener)o);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public int Add(IConversationListener o)
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
			return Contains((IConversationListener)o);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public bool Contains(IConversationListener o)
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
			return IndexOf((IConversationListener)o);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public int IndexOf(IConversationListener o)
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
			Insert(index, (IConversationListener)o);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="o"></param>
		public void Insert(int index, IConversationListener o)
		{
			this.arrayList.Insert(index, o);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		void IList.Remove(object o)
		{
			Remove((IConversationListener)o);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		public void Remove(IConversationListener o)
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
	/// Provides an observer for the "server side" - ie the UI side of NBM
	/// for conversations. This is not used by plugins.
	/// </summary>
	public class ConversationServer
	{
		private ConversationListenerCollection listenerList = new ConversationListenerCollection();
		private ProtocolServer protocolServer;


		/// <summary>
		/// Constructs a ConversationServer
		/// </summary>
		/// <param name="protocolServer">Protocol server to attach to</param>
		public ConversationServer(ProtocolServer protocolServer)
		{
			this.protocolServer = protocolServer;
		}


		#region Server methods


		/// <summary>
		/// Occurs when the conversation connects
		/// </summary>
		public void OnConnected()
		{
			foreach (IConversationListener listener in this.listenerList)
			{
				listener.OnConversationConnected();
			}
		}


		/// <summary>
		/// Occurs when the conversation disconnects
		/// </summary>
		public void OnDisconnected()
		{
			foreach (IConversationListener listener in this.listenerList)
			{
				listener.OnConversationDisconnected();
			}
		}


		#endregion


		#region Conversation control methods


		/// <summary>
		/// Add an observer to the conversation server
		/// </summary>
		/// <param name="listener"></param>
		public void AddListener(IConversationListener listener)
		{
			if (!this.listenerList.Contains(listener))
				listenerList.Add(listener);
		}


		/// <summary>
		/// Remove an observer from the conversation server
		/// </summary>
		/// <param name="listener"></param>
		public void RemoveListener(IConversationListener listener)
		{
			listenerList.Remove(listener);
		}


		/// <summary>
		/// A friend in the conversation has just said something
		/// </summary>
		/// <param name="username">Friend's username</param>
		/// <param name="text">Message said</param>
		public void FriendSay(string username, string text)
		{
			Friend friend = this.protocolServer.GetFriend(username);
			if (friend == null)
				friend = new Friend(username, OnlineStatus.Online, null);
			FriendSay(friend, text);
		}


		/// <summary>
		/// A friend in the conversation has just said something
		/// </summary>
		/// <param name="friend">Friend</param>
		/// <param name="text">Message said</param>
		public void FriendSay(Friend friend, string text)
		{
			foreach (IConversationListener listener in this.listenerList)
			{
				listener.OnFriendSay(friend, text);
			}
		}


		/// <summary>
		/// A friend has just joined the conversation
		/// </summary>
		/// <param name="username">Username of friend</param>
		public void FriendJoin(string username)
		{
			Friend friend = this.protocolServer.GetFriend(username);
			if (friend == null)
				friend = new Friend(username, OnlineStatus.Online, null);
			FriendJoin(friend);
		}


		/// <summary>
		/// A friend has just joined the conversation
		/// </summary>
		/// <param name="friend">Friend</param>
		public void FriendJoin(Friend friend)
		{
			foreach (IConversationListener listener in this.listenerList)
			{
				listener.OnFriendJoin(friend);
			}
		}


		/// <summary>
		/// Friend has just left the conversation
		/// </summary>
		/// <param name="username">Username of friend who has left</param>
		public void FriendLeave(string username)
		{
			Friend friend = this.protocolServer.GetFriend(username);
			if (friend == null)
				friend = new Friend(username, OnlineStatus.Online, null);
			FriendLeave(friend);
		}

		
		/// <summary>
		/// Friend has just left the conversation
		/// </summary>
		/// <param name="friend">Friend</param>
		public void FriendLeave(Friend friend)
		{
			foreach (IConversationListener listener in this.listenerList)
			{
				listener.OnFriendLeave(friend);
			}
		}


		/// <summary>
		/// Typing notification received by plugin
		/// </summary>
		/// <param name="username">Username of typer</param>
		public void TypingNotification(string username)
		{
			Friend friend = this.protocolServer.GetFriend(username);
			if (friend == null)
				friend = new Friend(username, OnlineStatus.Online, null);
			TypingNotification(friend);
		}


		/// <summary>
		/// Typing notification received by plugin
		/// </summary>
		/// <param name="friend">Friend typing</param>
		public void TypingNotification(Friend friend)
		{
			foreach (IConversationListener listener in this.listenerList)
			{
				listener.OnTypingNotification(friend);
			}
		}


		/// <summary>
		/// Creates a proxy connection
		/// </summary>
		/// <returns>Connection to use</returns>
		public Proxy.IConnection CreateConnection()
		{
			return this.protocolServer.CreateConnection();
		}


		/// <summary>
		/// Occurs when the user has said something
		/// </summary>
		/// <param name="text">Message to send to conversation</param>
		public void UserSay(string text)
		{
			foreach (IConversationListener listener in this.listenerList)
			{
				listener.OnUserSay(text);
			}
		}


		/// <summary>
		/// When a file invitation is received by another party
		/// </summary>
		/// <param name="friend">Friend who's sending the file</param>
		/// <param name="filename">Filename of file</param>
		public void FileSendInvitation(Friend friend, string filename)
		{
			foreach (IConversationListener listener in this.listenerList)
			{
				listener.OnFileSendInvitation(friend, filename);
			}
		}


		/// <summary>
		/// Call this when a file transfer invitation is received by someone else
		/// </summary>
		/// <param name="username">Friend who's sending the file</param>
		/// <param name="filename">Filename</param>
		public void FileSendInvitation(string username, string filename)
		{
			Friend friend = this.protocolServer.GetFriend(username);
			if (friend == null)
				friend = new Friend(username, OnlineStatus.Online, null);
			FileSendInvitation(friend, filename);
		}


		#endregion

	}
}
