namespace Rhino.Etl.Tests.Joins
{
    using Core.Operations;

    public class InnerMergeJoinUsersToPeopleByEmail : BaseMergeJoin
    {
        protected override JoinType JoinType
        {
            get { return JoinType.Inner; }
        }
    }
}