using System;
using System.Collections;

// 4/5/03

namespace NBM.Plugin
{
	/// <summary>
	/// Delegate to register with the OperationCompleteEvent class.
	/// </summary>
	public delegate void OperationCompleteHandler(OperationCompleteArgs args, object tag);


	/// <summary>
	/// Arguments to pass to OperationCompleteEvent.Execute() to report the success or failure
	/// of the specified operation
	/// <seealso cref="OperationCompleteEvent"/>
	/// </summary>
	public class OperationCompleteArgs
	{
		/// <summary>
		/// 
		/// </summary>
		protected bool success;

		/// <summary>
		/// 
		/// </summary>
		protected bool isFatal;

		/// <summary>
		/// 
		/// </summary>
		protected string errorMessage = string.Empty;


		/// <summary>
		/// True if operation was successful, false otherwise
		/// </summary>
		public bool Success
		{
			get { return success; }
		}


		/// <summary>
		/// Error message if the operation failed
		/// </summary>
		public string ErrorMessage
		{
			get { return errorMessage; }
		}


		/// <summary>
		/// Whether the error is fatal
		/// </summary>
		public bool IsFatalError
		{
			get { return isFatal; }
		}


		/// <summary>
		/// Constructs an OperationCompleteArgs reporting success
		/// </summary>
		public OperationCompleteArgs()
		{
			this.success = true;
		}


		/// <summary>
		/// Constructs an OperationCompleteArgs reporting that an error occurred
		/// </summary>
		/// <param name="errorMessage">Error message</param>
		/// <param name="isFatal">Whether the error is fatal</param>
		public OperationCompleteArgs(string errorMessage, bool isFatal)
		{
			this.success = false;
			this.errorMessage = errorMessage;
			this.isFatal = isFatal;
		}
	}


	/// <summary>
	/// Used by plugins to determine when an operation has completed.
	/// When an operation is finished, plugins should call Execute() and pass an
	/// OperationCompleteArgs instance which reports the success of the operation.
	/// </summary>
	public class OperationCompleteEvent
	{
		private Stack registeredEvents = new Stack();


		/// <summary>
		/// 
		/// </summary>
		public OperationCompleteEvent()
		{}


		/// <summary>
		///
		/// </summary>
		/// <param name="handler">Initial handler to register</param>
		public OperationCompleteEvent(OperationCompleteHandler handler)
			: this(handler, null)
		{
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="handler">Initial handler to register</param>
		/// <param name="tag">Object to pass to handler when executed</param>
		public OperationCompleteEvent(OperationCompleteHandler handler, object tag)
		{
			RegisterEvent(handler, tag);
		}


		/// <summary>
		/// Register a delegate to be executed when Execute() is called.
		/// Methods are executed first in last out.
		/// </summary>
		/// <param name="handler"></param>
		public void RegisterEvent(OperationCompleteHandler handler)
		{
			RegisterEvent(handler, null);
		}


		/// <summary>
		/// Register a delegate to be executed when Execute() is called.
		/// Methods are executed first in last out.
		/// </summary>
		/// <param name="handler"></param>
		/// <param name="tag">Object to pass to handler when executed</param>
		public void RegisterEvent(OperationCompleteHandler handler, object tag)
		{
			registeredEvents.Push(new Pair(handler, tag));
		}


		/// <summary>
		/// Execute all registered delegates and report either success or failure of the
		/// operation.
		/// </summary>
		/// <param name="args"></param>
		public void Execute(OperationCompleteArgs args)
		{
			int count = registeredEvents.Count;

			for (int i=0; i<count; ++i)
			{
				Pair pair = (Pair)registeredEvents.Pop();
				((OperationCompleteHandler)pair.First)(args, pair.Second);
			}
		}
	}
}