namespace Rhino.Etl.Tests.LoadTest
{
    using System.Data;
    using Commons;
    using Core;
    using MbUnit.Framework;
    using Rhino.Etl.Core.Operations;

    /// <summary>
    /// This fixture is here to verify that we can handle large amount of data
    /// without consuming too much memory or crashing
    /// </summary>
    [TestFixture(TimeOut = 120)]
    public class LoadTestFixture : BaseUserToPeopleTest
    {
        private const int expectedCount = 5000;
        private int currentUserCount;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
            currentUserCount = GetUserCount("1 = 1");
            using (PushDataToDatabase push = new PushDataToDatabase(expectedCount))
                push.Execute();
        }

        public void AssertUpdatedAllRows()
        {
            Assert.AreEqual(expectedCount + currentUserCount, GetUserCount("testMsg is not null"));

        }

        private static int GetUserCount(string where)
        {
            return Use.Transaction<int>("test", delegate(IDbCommand command)
            {
                command.CommandText = "select count(*) from users where " + where;
                return (int)command.ExecuteScalar();
            });
        }

        [Test]
        public void CanUpdateAllUsersToUpperCase()
        {
            using (UpperCaseUserNames update = new UpperCaseUserNames())
            {
                update.RegisterLast(new UpdateUserNames());
                update.Execute();
            }
            AssertUpdatedAllRows();
        }

        [Test]
        public void CanBatchUpdateAllUsersToUpperCase()
        {
            using (UpperCaseUserNames update = new UpperCaseUserNames())
            {
                update.RegisterLast(new BatchUpdateUserNames());
                update.Execute();
            }

            AssertUpdatedAllRows();
        }

        [Test]
        public void BulkInsertUpdatedRows()
        {
            if(expectedCount != GetUserCount("1 = 1"))
                Assert.Ignore("That is _really_ strange");

            using (UpperCaseUserNames update = new UpperCaseUserNames())
            {
                update.RegisterLast(new BulkInsertUsers());
                update.Execute();
            }

            AssertUpdatedAllRows();
        }
    }
}