namespace Rhino.Etl.Tests.Joins
{
    using MbUnit.Framework;

    [TestFixture]
    public class JoinInProcessFixture : BaseJoinFixture
    {
        [Test]
        public void CanUseJoinInProcess()
        {
            using (TrivialUsersToPeopleJoinProcess process = new TrivialUsersToPeopleJoinProcess(left, right))
            {
                process.Execute();
                Assert.AreEqual(1, process.Results.Count);
                Assert.AreEqual(3, process.Results[0]["person_id"]);
            }
        }

        [Test]
        public void CanUseComplexJoinInProcesses()
        {
            using (ComplexUsersToPeopleJoinProcess process = new ComplexUsersToPeopleJoinProcess(left, right))
            {
                process.Execute();
                Assert.AreEqual(2, process.Results.Count);
                Assert.AreEqual("FOO", process.Results[0]["name"]);
                Assert.AreEqual("FOO@EXAMPLE.ORG", process.Results[0]["email"]);
            }
        }
    }
}