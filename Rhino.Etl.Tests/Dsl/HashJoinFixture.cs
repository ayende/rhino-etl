using Rhino.Etl.Core.Infrastructure;

namespace Rhino.Etl.Tests.Dsl
{
    using System.Collections.Generic;
    using System.Data;
    using Core;
    using Xunit;
    using Rhino.Etl.Dsl;

    
    public class HashJoinFixture : BaseUserToPeopleTest
    {
        [Fact]
        public void CanCompile()
        {
            using(EtlProcess process = CreateDslInstance("Dsl/InnerHashJoin.boo"))
                Assert.NotNull(process);
        }

        [Fact]
        public void CanWriteJoinsToDatabase()
        {
            using(EtlProcess process = CreateDslInstance("Dsl/InnerHashJoin.boo"))
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
            Assert.Equal("ayende rahien is: admin, janitor, employee, customer", roles[0]);
            Assert.Equal("foo bar is: janitor", roles[1]);
            Assert.Equal("gold silver is: janitor, employee", roles[2]);
        }
    }
}