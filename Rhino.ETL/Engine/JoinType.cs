using System;

namespace Rhino.ETL
{
	[Flags]
	public enum JoinType
	{
		Inner = 1,
		Left = 2,
		Right = 4,
		Full = 6 //both left and right
	}
}