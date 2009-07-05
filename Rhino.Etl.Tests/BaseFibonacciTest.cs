using System.Data;
using MbUnit.Framework;
using Rhino.Commons;

namespace Rhino.Etl.Tests
{
    public class BaseFibonacciTest
    {
        [SetUp]
        public void SetUp()
        {
            Use.Transaction("test", delegate(IDbCommand cmd)
            {
                cmd.CommandText =
                    @"
if object_id('Fibonacci') is not null
    drop table Fibonacci
create table Fibonacci ( id int );
";
                cmd.ExecuteNonQuery();
            });
        }

        protected static void Assert25ThFibonacci()
        {
            int max = Use.Transaction("test", delegate(IDbCommand cmd)
            {
                cmd.CommandText = "SELECT MAX(id) FROM Fibonacci";
                return (int) cmd.ExecuteScalar();
            });
            Assert.AreEqual(75025, max);
        }

        protected static void AssertFibonacciTableEmpty()
        {
            int count = Use.Transaction("test", delegate(IDbCommand cmd)
            {
                cmd.CommandText = "SELECT count(id) FROM Fibonacci";
                return (int) cmd.ExecuteScalar();
            });
            Assert.AreEqual(0, count);
        }
    }
}