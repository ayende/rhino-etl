namespace Rhino.Etl.Tests.Joins
{
    using Core;

    public class InnerJoinUsersToPeopleByEmail : BaseJoinUsersToPeople
    {
        protected override bool MatchJoinCondition(Row leftRow, Row rightRow)
        {
            return Equals(leftRow["email"], rightRow["email"]);
        }
    }
}