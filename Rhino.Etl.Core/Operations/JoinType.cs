namespace Rhino.Etl.Core.Operations
{
	using System;

	/// <summary>
	/// Define the supported join types
	/// </summary>
	[Flags]
	public enum JoinType
	{
		/// <summary>
		/// Inner join
		/// </summary>
		Inner = 0,
		/// <summary>
		/// Left outer join
		/// </summary>
		Left = 1,
		/// <summary>
		/// Right outer join
		/// </summary>
		Right = 2,
		/// <summary>
		/// Full outer join
		/// </summary>
		Full = Left | Right
	}
}