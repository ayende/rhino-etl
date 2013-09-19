namespace Rhino.Etl.Tests.Joins
{
    using Core.Operations;

    public class RightMergeJoinUsersToPeopleByEmail : BaseMergeJoin
    {
        protected override JoinType JoinType
        {
            get { return JoinType.Right; }
        }
    }
}