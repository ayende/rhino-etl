namespace Rhino.Etl.Tests
{
    using System;
    using System.Collections.Generic;
    using Xunit;
    using Rhino.Etl.Tests.Fibonacci.Bulk;
    using Rhino.Etl.Tests.Fibonacci.Output;

    
    public class SqlBulkInsertOperationFixture : BaseFibonacciTest
    {
        [Fact]
        public void CanInsertToDatabaseFromInMemoryCollection()
        {
            BulkInsertFibonacciToDatabase fibonacci = new BulkInsertFibonacciToDatabase(25,Should.WorkFine);
            fibonacci.Execute();

            Assert25ThFibonacci();
        }


        [Fact]
        public void WhenErrorIsThrownWillRollbackTransaction()
        {
            BulkInsertFibonacciToDatabase fibonaci = new BulkInsertFibonacciToDatabase(25, Should.Throw);
            fibonaci.Execute();
            Assert.Equal(1, new List<Exception>(fibonaci.GetAllErrors()).Count);
            AssertFibonacciTableEmpty();
        }
    }
}
