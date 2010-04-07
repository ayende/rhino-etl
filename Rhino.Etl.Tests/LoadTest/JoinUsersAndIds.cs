namespace Rhino.Etl.Tests.LoadTest
{
	using Core;
	using Rhino.Etl.Core.Operations;

	public class JoinUsersAndIds : JoinOperation
	{
		protected override Row MergeRows(Row leftRow, Row rightRow)
		{
			Row row = leftRow.Clone();
			row["user_id"] = rightRow["new_id"];
			return row;
		}

		protected override void SetupJoinConditions()
		{
			InnerJoin
				.Left("id")
				.Right("old_id");
		}
	}
}