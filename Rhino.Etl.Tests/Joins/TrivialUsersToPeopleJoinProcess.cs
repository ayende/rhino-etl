namespace Rhino.Etl.Tests.Joins
{
    using System.Collections.Generic;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class TrivialUsersToPeopleJoinProcess : EtlProcess
    {
        private readonly IEnumerable<Row> left;
        private readonly IEnumerable<Row> right;

        public List<Row> Results = new List<Row>();

        public TrivialUsersToPeopleJoinProcess(IEnumerable<Row> left, IEnumerable<Row> right)
        {
            this.left = left;
            this.right = right;
        }

        protected override void Initialize()
        {
            Register(new InnerJoinUsersToPeopleByEmail()
                         .Left(new GenericEnumerableOperation(left))
                         .Right(new GenericEnumerableOperation(right))
                );
            Register(new AddToResults(this.Results));
        }
    }
}