namespace Rhino.Etl.Tests.Integration
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Data;
    using Boo.Lang;
    using Commons;
    using Core;
    using MbUnit.Framework;
    using Rhino.Etl.Core.Operations;

    [TestFixture]
    public class DatabaseToDatabaseWithTransformations : BaseUserToPeopleTest
    {
        [Test]
        public void CanCopyTableWithTransform()
        {
            using(UsersToPeople process = new UsersToPeople())
                process.Execute();
            

            System.Collections.Generic.List<string[]> names = Use.Transaction<System.Collections.Generic.List<string[]>>("test", delegate(IDbCommand cmd)
            {
                System.Collections.Generic.List<string[]> tuples = new System.Collections.Generic.List<string[]>();
                cmd.CommandText = "SELECT firstname, lastname from people order by userid";
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        tuples.Add(new string[] { reader.GetString(0), reader.GetString(1) });
                    }
                }
                return tuples;
            });
            AssertNames(names);
        }
    }
}