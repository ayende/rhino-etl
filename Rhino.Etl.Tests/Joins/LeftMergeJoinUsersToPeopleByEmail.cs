namespace Rhino.Etl.Tests.Joins
{
    using Core.Operations;

    public class LeftMergeJoinUsersToPeopleByEmail : BaseMergeJoin
    {
        protected override JoinType JoinType
        {
            get { return JoinType.Left; }
        }
    }
}