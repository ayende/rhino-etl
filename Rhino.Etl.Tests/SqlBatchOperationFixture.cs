namespace Rhino.Etl.Tests
{
    using System;
    using System.Collections.Generic;
    using Xunit;
    using Rhino.Etl.Tests.Fibonacci.Batch;
    using Rhino.Etl.Tests.Fibonacci.Output;

    
    public class SqlBatchOperationFixture : BaseFibonacciTest
    {
        [Fact]
        public void CanInsertToDatabaseFromInMemoryCollection()
        {
            BatchFibonacci fibonaci = new BatchFibonacci(25,Should.WorkFine);
            fibonaci.Execute();

            Assert25ThFibonacci();
        }

        [Fact]
        public void CanInsertToDatabaseFromInMemoryCollectionWithSlowOperation()
        {
            var fibonaci = new SlowBatchFibonacci(25, Should.WorkFine);
            fibonaci.Execute();

            Assert25ThFibonacci();
        }

        [Fact]
        public void CanInsertToDatabaseFromConnectionStringSettingsAndInMemoryCollection()
        {
            BatchFibonacciFromConnectionStringSettings fibonaci = new BatchFibonacciFromConnectionStringSettings(25, Should.WorkFine);
            fibonaci.Execute();

            Assert25ThFibonacci();
        }

        [Fact]
        public void WhenErrorIsThrownWillRollbackTransaction()
        {
            BatchFibonacci fibonaci = new BatchFibonacci(25, Should.Throw);
            fibonaci.Execute();
            Assert.Equal(1, new List<Exception>(fibonaci.GetAllErrors()).Count);
            AssertFibonacciTableEmpty();
        }
    }
}