using System;

// 4/5/03

namespace NBM.Plugin
{
	/// <summary>
	/// Very simple - just stores a pair of objects.
	/// </summary>
	public class Pair
	{
		/// <summary>
		/// 
		/// </summary>
		public object First;

		/// <summary>
		/// 
		/// </summary>
		public object Second;


		/// <summary>
		/// Constructs a pair with null as the default values
		/// </summary>
		public Pair()
		{
			First = Second = null;
		}


		/// <summary>
		/// Constructs a pair with the specified object values.
		/// </summary>
		/// <param name="first"></param>
		/// <param name="second"></param>
		public Pair(object first, object second)
		{
			First = first;
			Second = second;
		}
	}
}