namespace Rhino.Etl.Tests.Branches
{
	using System.Collections.Generic;
	using Core;
	using Rhino.Etl.Core.Operations;

	internal class MultiplyByThreeOperation : AbstractOperation
	{
		public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
		{
			foreach (Row row in rows)
			{
				row["id"] = 3*(int)row["id"];
				yield return row;
			}
		}
	}
}