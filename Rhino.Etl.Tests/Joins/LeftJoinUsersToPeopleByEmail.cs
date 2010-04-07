namespace Rhino.Etl.Tests.Joins
{
    using System.Collections.Generic;
    using Core;

    public class LeftJoinUsersToPeopleByEmail : BaseJoinUsersToPeople
    {
        protected override bool MatchJoinCondition(Row leftRow, Row rightRow)
        {
            return Equals(leftRow["email"], rightRow["email"]) || rightRow["email"] == null;
        }
    }
}