namespace Rhino.Etl.Tests.Dsl
{
    using System.Collections.Generic;
    using System.Data;
    using Commons;
    using Core;
    using MbUnit.Framework;
    using Rhino.Etl.Dsl;

    [TestFixture]
    public class JoinFixture : BaseUserToPeopleTest
    {
        [Test]
        public void CanCompile()
        {
            using(EtlProcess process = CreateDslInstance("Dsl/InnerJoin.boo"))
                Assert.IsNotNull(process);
        }

        [Test]
        public void CanWriteJoinsToDatabase()
        {
            using(EtlProcess process = CreateDslInstance("Dsl/InnerJoin.boo"))
                process.Execute();
            List<string> roles = new List<string>();
            Use.Transaction("test", delegate(IDbCommand command)
            {
                command.CommandText = @"
                                SELECT Roles FROM Users
                                WHERE Roles IS NOT NULL
                                ORDER BY Id
                ";
                using(IDataReader reader = command.ExecuteReader())
                while(reader.Read())
                {
                    roles.Add(reader.GetString(0));
                }
            });
            Assert.AreEqual("ayende rahien is: admin, janitor, employee, customer", roles[0]);
            Assert.AreEqual("foo bar is: janitor", roles[1]);
            Assert.AreEqual("gold silver is: janitor, employee", roles[2]);
        }
    }
}