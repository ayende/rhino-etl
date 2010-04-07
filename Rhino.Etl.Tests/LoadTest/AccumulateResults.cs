namespace Rhino.Etl.Tests.LoadTest
{
	using System.Collections.Generic;
	using Core;
	using Rhino.Etl.Core.Operations;

	public class AccumulateResults : AbstractOperation
	{
		public int count = 0;

		public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
		{
			foreach (Row row in rows)
			{
				count += 1;
			}
			yield break;
		}
	}
}