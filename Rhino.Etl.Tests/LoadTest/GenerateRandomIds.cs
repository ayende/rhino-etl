namespace Rhino.Etl.Tests.LoadTest
{
	using System;
	using System.Collections.Generic;
	using Core;
	using Rhino.Etl.Core.Operations;

	public class GenerateRandomIds : AbstractOperation
	{
		public GenerateRandomIds(int expectedCount)
		{
			this.expectedCount = expectedCount;
		}

		private readonly int expectedCount;
		public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
		{
			for (int i = 0; i < expectedCount; i++)
			{
				Row row = new Row();
				row["old_id"] = i;
				row["new_id"] = Guid.NewGuid();
				yield return row;
			}
		}
	}
}