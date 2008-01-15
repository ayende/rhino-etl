namespace Rhino.Etl.Tests
{
    using System;
    using System.Collections.Generic;
    using MbUnit.Framework;
    using Rhino.Etl.Tests.Fibonacci.Bulk;
    using Rhino.Etl.Tests.Fibonacci.Output;

    [TestFixture]
    public class SqlBulkInsertOperationFixture : BaseFibonacciTest
    {
        [Test]
        public void CanInsertToDatabaseFromInMemoryCollection()
        {
            BulkInsertFibonacciToDatabase fibonacci = new BulkInsertFibonacciToDatabase(25,Should.WorkFine);
            fibonacci.Execute();

            Assert25ThFibonacci();
        }


        [Test]
        public void WhenErrorIsThrownWillRollbackTransaction()
        {
            BulkInsertFibonacciToDatabase fibonaci = new BulkInsertFibonacciToDatabase(25, Should.Throw);
            fibonaci.Execute();
            Assert.AreEqual(1, new List<Exception>(fibonaci.GetAllErrors()).Count);
            AssertFibonacciTableEmpty();
        }
    }
}
