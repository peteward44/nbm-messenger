using System;
using System.Collections;

using System.Runtime.Serialization;

// 1/6/03

namespace NBM.Plugin
{
	/// <summary>
	/// Represents a friend on a protocol's contact list.
	/// </summary>
	[Serializable()]
	public class Friend : ISerializable
	{
		private string displayName = string.Empty;
		private string username = string.Empty;
		private string emailAddress = string.Empty;
		private bool blocked = false;
		private OnlineStatus status = OnlineStatus.Offline;
		private object groupIdentifier = null;
		private ArrayList listeners = new ArrayList();


		/// <summary>
		/// Plugin-defined group identifier
		/// </summary>
		public object GroupIdentifier
		{
			get { return this.groupIdentifier; }
			set { this.groupIdentifier = value; }
		}


		/// <summary>
		/// Blocked flag
		/// </summary>
		public bool Blocked
		{
			get { return this.blocked; }
			set { this.blocked = value; }
		}


		/// <summary>
		/// Adds a listener to the friend events
		/// </summary>
		/// <param name="listener"></param>
		internal void AddListener(IProtocolListener listener)
		{
			if (!this.listeners.Contains(listener))
				this.listeners.Add(listener);
		}


		/// <summary>
		/// Removes a listener from the friend events
		/// </summary>
		/// <param name="listener"></param>
		internal void RemoveListener(IProtocolListener listener)
		{
			this.listeners.Remove(listener);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		/// <returns></returns>
		public override bool Equals(object o)
		{
			if (o == null)
				return false;
			else if (!(o is Friend))
				throw new ApplicationException();
			else
				return ((Friend)o).username == this.username;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return username.GetHashCode();
		}


		/// <summary>
		/// Friend's display name to show on the contact list
		/// Will default to the username if this is not assigned
		/// </summary>
		public string DisplayName
		{
			get { return (displayName == string.Empty) ? username : displayName; }
			set
			{
				if (displayName != value)
				{
					foreach (IProtocolListener listener in this.listeners)
					{
						listener.OnFriendChangeDisplayName(this, value);
					}

					displayName = value;
				}
			}
		}


		/// <summary>
		/// Friend's username
		/// </summary>
		public string Username
		{
			get { return username; }
		}


		/// <summary>
		/// Friend's email address
		/// </summary>
		public string EmailAddress
		{
			get { return emailAddress; }
			set { emailAddress = value; }
		}


		/// <summary>
		/// Friend's online status
		/// </summary>
		public OnlineStatus Status
		{
			get { return status; }
			set
			{
				if (status != value)
				{
					foreach (IProtocolListener listener in this.listeners)
					{
						listener.OnFriendChangeStatus(this, value);
					}

					status = value;
				}
			}
		}


		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return DisplayName;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="ctxt"></param>
		public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
		{
			info.AddValue("DisplayName", this.displayName);
			info.AddValue("Username", this.username);
			info.AddValue("EmailAddress", this.emailAddress);
			info.AddValue("Blocked", this.blocked);
			info.AddValue("GroupType", this.groupIdentifier.GetType().FullName);
			info.AddValue("Group", this.groupIdentifier);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="ctxt"></param>
		public Friend(SerializationInfo info, StreamingContext ctxt)
		{
			this.displayName = info.GetString("DisplayName");
			this.username = info.GetString("Username");
			this.emailAddress = info.GetString("EmailAddress");
			this.blocked = info.GetBoolean("Blocked");
			
			string groupType = info.GetString("GroupType");
			this.groupIdentifier = info.GetValue("Group", Type.GetType(groupType));

			this.status = OnlineStatus.Offline;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"></param>
		/// <param name="status"></param>
		public Friend(string username, OnlineStatus status)
		{
			this.username = username;
			this.status = status;
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="username"></param>
		/// <param name="status"></param>
		/// <param name="groupID"></param>
		public Friend(string username, OnlineStatus status, object groupID)
			: this(username, status)
		{
			this.groupIdentifier = groupID;
		}
	}
}
