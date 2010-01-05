namespace Rhino.Etl.Tests.Joins
{
    using System.Collections.Generic;
    using Core;
    using Xunit;
    using Rhino.Etl.Core.Pipelines;

    
    public class JoinFixture : BaseJoinFixture
    {
      
        [Fact]
        public void InnerJoin()
        {
            using (InnerJoinUsersToPeopleByEmail join = new InnerJoinUsersToPeopleByEmail())
            {
                join.Left(new GenericEnumerableOperation(left))
                    .Right(new GenericEnumerableOperation(right));
                join.PrepareForExecution(new SingleThreadedPipelineExecuter());
                IEnumerable<Row> result = join.Execute(null);
                List<Row> items = new List<Row>(result);

                Assert.Equal(1, items.Count);
                Assert.Equal(3, items[0]["person_id"]);
            }
        }

        [Fact]
        public void RightJoin()
        {
            using (RightJoinUsersToPeopleByEmail join = new RightJoinUsersToPeopleByEmail())
            {
                join.Left(new GenericEnumerableOperation(left))
                    .Right(new GenericEnumerableOperation(right));
                join.PrepareForExecution(new SingleThreadedPipelineExecuter());
                IEnumerable<Row> result = join.Execute(null);
                List<Row> items = new List<Row>(result);

                Assert.Equal(2, items.Count);
                Assert.Equal(3, items[0]["person_id"]);
                Assert.Null(items[1]["name"]);
                Assert.Equal(5, items[1]["person_id"]);
            }
        }

        [Fact]
        public void LeftJoin()
        {
            using (LeftJoinUsersToPeopleByEmail join = new LeftJoinUsersToPeopleByEmail())
            {
                join.Left(new GenericEnumerableOperation(left))
                    .Right(new GenericEnumerableOperation(right));
                join.PrepareForExecution(new SingleThreadedPipelineExecuter());
                IEnumerable<Row> result = join.Execute(null);
                List<Row> items = new List<Row>(result);

                Assert.Equal(2, items.Count);
                Assert.Equal(3, items[0]["person_id"]);
                Assert.Null(items[1]["person_id"]);
                Assert.Equal("bar", items[1]["name"]);
            }
        }

        [Fact]
        public void FullJoin()
        {
            using (FullJoinUsersToPeopleByEmail join = new FullJoinUsersToPeopleByEmail())
            {
                join.Left(new GenericEnumerableOperation(left))
                    .Right(new GenericEnumerableOperation(right));
                join.PrepareForExecution(new SingleThreadedPipelineExecuter());
                IEnumerable<Row> result = join.Execute(null);
                List<Row> items = new List<Row>(result);

                Assert.Equal(3, items.Count);

                Assert.Equal(3, items[0]["person_id"]);

                Assert.Null(items[1]["person_id"]);
                Assert.Equal("bar", items[1]["name"]);

                Assert.Null(items[2]["name"]);
                Assert.Equal(5, items[2]["person_id"]);
            }
        }
    }
}