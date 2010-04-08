namespace Rhino.Etl.Tests.Joins
{
    using System.Collections.Generic;
    using Core;
    using Rhino.Etl.Core.Operations;

    public abstract class BaseJoinUsersToPeople : NestedLoopsJoinOperation
    {
        protected override Row MergeRows(Row leftRow, Row rightRow)
        {
            Row row = new Row();
            row.Copy(leftRow);
            row["person_id"] = rightRow["id"];
            return row;
        }
    }
}