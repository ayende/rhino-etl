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

        [Fact]
        public void SortMergeInnerJoin()
        {
            using (InnerMergeJoinUsersToPeopleByEmail join = new InnerMergeJoinUsersToPeopleByEmail())
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
        public void SortMergeLeftJoin()
        {
            using (LeftMergeJoinUsersToPeopleByEmail join = new LeftMergeJoinUsersToPeopleByEmail())
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
        public void SortMergeRightJoin()
        {
            using (RightMergeJoinUsersToPeopleByEmail join = new RightMergeJoinUsersToPeopleByEmail())
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
        public void SortMergeFullJoin()
        {
            using (FullMergeJoinUsersToPeopleByEmail join = new FullMergeJoinUsersToPeopleByEmail())
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

        [Fact]
        public void SortMergeFullJoinMergesMultipleNonMatchingLeftRowsAtEnd()
        {
            AddUser("toby", "toby@test.org");
            AddUser("tom", "tom@test.org");

            using (FullMergeJoinUsersToPeopleByEmail join = new FullMergeJoinUsersToPeopleByEmail())
            {
                join.Left(new GenericEnumerableOperation(left))
                    .Right(new GenericEnumerableOperation(right));
                join.PrepareForExecution(new SingleThreadedPipelineExecuter());
                IEnumerable<Row> result = join.Execute(null);
                List<Row> items = new List<Row>(result);

                Assert.Equal(5, items.Count);
                Assert.Null(items[3]["person_id"]);
                Assert.Equal("toby", items[3]["name"]);

                Assert.Null(items[4]["person_id"]);
                Assert.Equal("tom", items[4]["name"]);
            }
        }

        [Fact]
        public void SortMergeFullJoinMergesMultipleNonMatchingRightRowsAtEnd()
        {
            AddPerson(8, "toby@test.org");
            AddPerson(10, "tom@test.org");

            using (FullMergeJoinUsersToPeopleByEmail join = new FullMergeJoinUsersToPeopleByEmail())
            {
                join.Left(new GenericEnumerableOperation(left))
                    .Right(new GenericEnumerableOperation(right));
                join.PrepareForExecution(new SingleThreadedPipelineExecuter());
                IEnumerable<Row> result = join.Execute(null);
                List<Row> items = new List<Row>(result);

                Assert.Equal(5, items.Count);
                Assert.Equal(8, items[3]["person_id"]);
                Assert.Null(items[3]["name"]);

                Assert.Equal(10, items[4]["person_id"]);
                Assert.Null(items[4]["name"]);
            }
        }
    }
}