using System;
using System.Threading;
using System.Globalization;

// 14/6/03

namespace NBM.Plugin
{
	/// <summary>
	/// Just like System.Thread, except we can pass arguments to it.
	/// <seealso cref="System.Threading.Thread"/>
	/// </summary>
	public class ArgThread
	{
		private Delegate dele;
		private object[] args;
		private Thread thread;


		#region Thread class properties


		/// <summary>
		/// 
		/// </summary>
		public ApartmentState ApartmentState
		{
			get { return thread.ApartmentState; }
			set { thread.ApartmentState = value; }
		}


		/// <summary>
		/// 
		/// </summary>
		public CultureInfo CurrentCulture
		{
			get { return thread.CurrentCulture; }
			set { thread.CurrentCulture = value; }
		}


		/// <summary>
		/// 
		/// </summary>
		public CultureInfo CurrentUICulture
		{
			get { return thread.CurrentUICulture; }
			set { thread.CurrentUICulture = value; }
		}


		/// <summary>
		/// 
		/// </summary>
		public bool IsAlive
		{
			get { return thread.IsAlive; }
		}


		/// <summary>
		/// 
		/// </summary>
		public bool IsBackground
		{
			get { return thread.IsBackground; }
			set { thread.IsBackground = value; }
		}


		/// <summary>
		/// 
		/// </summary>
		public bool IsThreadPoolThread
		{
			get { return thread.IsThreadPoolThread; }
		}


		/// <summary>
		/// 
		/// </summary>
		public string Name
		{
			get { return thread.Name; }
			set { thread.Name = value; }
		}


		/// <summary>
		/// 
		/// </summary>
		public ThreadPriority Priority
		{
			get { return thread.Priority; }
			set { thread.Priority = value; }
		}


		/// <summary>
		/// 
		/// </summary>
		public ThreadState ThreadState
		{
			get { return thread.ThreadState; }
		}


		#endregion


		/// <summary>
		/// Constructs an ArgThread.
		/// </summary>
		/// <param name="dele">Delegate to execute on Start()</param>
		public ArgThread(Delegate dele)
		{
			this.dele = dele;
			this.thread = new Thread(new ThreadStart(OnThreadStart));
		}


		/// <summary>
		/// Internal method that is passed to Thread.Start()
		/// </summary>
		private void OnThreadStart()
		{
			this.dele.DynamicInvoke(args);
		}


		#region Thread class methods


		/// <summary>
		/// 
		/// </summary>
		public void Abort()
		{
			this.thread.Abort();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="stateInfo"></param>
		public void Abort(object stateInfo)
		{
			this.thread.Abort(stateInfo);
		}


		/// <summary>
		/// 
		/// </summary>
		public void Interrupt()
		{
			this.thread.Interrupt();
		}


		/// <summary>
		/// 
		/// </summary>
		public void Join()
		{
			this.thread.Join();
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="millisecondsTimeout"></param>
		/// <returns></returns>
		public bool Join(int millisecondsTimeout)
		{
			return this.thread.Join(millisecondsTimeout);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <returns></returns>
		public bool Join(TimeSpan timeSpan)
		{
			return this.thread.Join(timeSpan);
		}


		/// <summary>
		/// 
		/// </summary>
		public void Resume()
		{
			this.thread.Resume();
		}


		/// <summary>
		/// 
		/// </summary>
		public void Suspend()
		{
			this.thread.Suspend();
		}


		/// <summary>
		/// 
		/// </summary>
		public void Start()
		{
			Start(null);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="args"></param>
		public void Start(object[] args)
		{
			this.args = args;
			thread.Start();
		}


		#endregion

	}
}
