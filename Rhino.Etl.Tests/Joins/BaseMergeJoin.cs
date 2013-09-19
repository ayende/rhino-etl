namespace Rhino.Etl.Tests.Joins
{
    using Core;
    using Rhino.Etl.Core.Operations;

    public abstract class BaseMergeJoin : SortMergeJoinOperation
    {
        protected override Row MergeRows(Row leftRow, Row rightRow)
        {
            Row row = new Row();
            row.Copy(leftRow);
            row["person_id"] = rightRow["id"];
            return row;
        }

        protected override int MatchJoinCondition(Row leftRow, Row rightRow)
        {
            return string.Compare((string)leftRow["email"], (string)rightRow["email"]);
        }
    }
}