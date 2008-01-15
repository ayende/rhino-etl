namespace Rhino.Etl.Tests.Joins
{
    using System.Collections.Generic;
    using Core;
    using MbUnit.Framework;
    using Rhino.Etl.Core.Pipelines;

    [TestFixture]
    public class JoinFixture : BaseJoinFixture
    {
      
        [Test]
        public void InnerJoin()
        {
            using (InnerJoinUsersToPeopleByEmail join = new InnerJoinUsersToPeopleByEmail())
            {
                join.Left(new GenericEnumerableOperation(left))
                    .Right(new GenericEnumerableOperation(right));
                join.PrepareForExecution(new SingleThreadedPipelineExecuter());
                IEnumerable<Row> result = join.Execute(null);
                List<Row> items = new List<Row>(result);

                Assert.AreEqual(1, items.Count);
                Assert.AreEqual(3, items[0]["person_id"]);
            }
        }

        [Test]
        public void RightJoin()
        {
            using (RightJoinUsersToPeopleByEmail join = new RightJoinUsersToPeopleByEmail())
            {
                join.Left(new GenericEnumerableOperation(left))
                    .Right(new GenericEnumerableOperation(right));
                join.PrepareForExecution(new SingleThreadedPipelineExecuter());
                IEnumerable<Row> result = join.Execute(null);
                List<Row> items = new List<Row>(result);

                Assert.AreEqual(2, items.Count);
                Assert.AreEqual(3, items[0]["person_id"]);
                Assert.IsNull(items[1]["name"]);
                Assert.AreEqual(5, items[1]["person_id"]);
            }
        }

        [Test]
        public void LeftJoin()
        {
            using (LeftJoinUsersToPeopleByEmail join = new LeftJoinUsersToPeopleByEmail())
            {
                join.Left(new GenericEnumerableOperation(left))
                    .Right(new GenericEnumerableOperation(right));
                join.PrepareForExecution(new SingleThreadedPipelineExecuter());
                IEnumerable<Row> result = join.Execute(null);
                List<Row> items = new List<Row>(result);

                Assert.AreEqual(2, items.Count);
                Assert.AreEqual(3, items[0]["person_id"]);
                Assert.IsNull(items[1]["person_id"]);
                Assert.AreEqual("bar", items[1]["name"]);
            }
        }

        [Test]
        public void FullJoin()
        {
            using (FullJoinUsersToPeopleByEmail join = new FullJoinUsersToPeopleByEmail())
            {
                join.Left(new GenericEnumerableOperation(left))
                    .Right(new GenericEnumerableOperation(right));
                join.PrepareForExecution(new SingleThreadedPipelineExecuter());
                IEnumerable<Row> result = join.Execute(null);
                List<Row> items = new List<Row>(result);

                Assert.AreEqual(3, items.Count);

                Assert.AreEqual(3, items[0]["person_id"]);

                Assert.IsNull(items[1]["person_id"]);
                Assert.AreEqual("bar", items[1]["name"]);

                Assert.IsNull(items[2]["name"]);
                Assert.AreEqual(5, items[2]["person_id"]);
            }
        }
    }
}