namespace Rhino.Etl.Tests
{
    using System;
    using System.Collections.Generic;
    using MbUnit.Framework;
    using Rhino.Etl.Tests.Fibonacci.Output;

    [TestFixture]
    public class OutputCommandFixture : BaseFibonacciTest
    {
        [Test]
        public void CanInsertToDatabaseFromInMemoryCollection()
        {
            OutputFibonacciToDatabase fibonaci = new OutputFibonacciToDatabase(25,Should.WorkFine);
            fibonaci.Execute();

            Assert25ThFibonacci();
        }

        [Test]
        public void WhenErrorIsThrownWillRollbackTransaction()
        {
            OutputFibonacciToDatabase fibonaci = new OutputFibonacciToDatabase(25, Should.Throw);
            fibonaci.Execute();
            Assert.AreEqual(1, new List<Exception>(fibonaci.GetAllErrors()).Count);
            AssertFibonacciTableEmpty();
        }
    }
}