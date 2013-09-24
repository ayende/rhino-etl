namespace Rhino.Etl.Tests.Joins
{
    using Core.Operations;

    public class FullMergeJoinUsersToPeopleByEmail : BaseMergeJoin
    {
        protected override JoinType JoinType
        {
            get { return JoinType.Full; }
        }
    }
}