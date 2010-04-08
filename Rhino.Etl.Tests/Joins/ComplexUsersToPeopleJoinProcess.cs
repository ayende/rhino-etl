namespace Rhino.Etl.Tests.Joins
{
    using System.Collections.Generic;
    using Core;

    public class ComplexUsersToPeopleJoinProcess : EtlProcess
    {
        private readonly IEnumerable<Row> left;
        private readonly IEnumerable<Row> right;

        public List<Row> Results = new List<Row>();

        public ComplexUsersToPeopleJoinProcess(IEnumerable<Row> left, IEnumerable<Row> right)
        {
            this.left = left;
            this.right = right;
        }

        protected override void Initialize()
        {
            Register(
                new RightJoinUsersToPeopleByEmail()
                         .Left(Partial
                                   .Register(new GenericEnumerableOperation(left))
                                   .Register(new AllStringsToUpperCase()))
                         .Right(Partial
                                    .Register(new GenericEnumerableOperation(right))
                                    .Register(new AllStringsToUpperCase()))
            );
            Register(new AddToResults(Results));
        }
    }
}