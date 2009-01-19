namespace Rhino.Etl.Tests
{
    using System;
    using System.Collections.Generic;
    using MbUnit.Framework;
    using Fibonacci.Output;

    [TestFixture]
    public class OutputCommandFixture : BaseFibonacciTest
    {
        [Test]
        public void CanInsertToDatabaseFromInMemoryCollection()
        {
            OutputFibonacciToDatabase fibonaci = new OutputFibonacciToDatabase(25, Should.WorkFine);
            fibonaci.Execute();

            Assert25ThFibonacci();
        }

        [Test]
        public void WillRaiseRowProcessedEvent()
        {
            int rowsProcessed = 0;

            using (OutputFibonacciToDatabase fibonaci = new OutputFibonacciToDatabase(1, Should.WorkFine))
            {
                fibonaci.OutputOperation.OnRowProcessed += delegate { rowsProcessed++; };
                fibonaci.Execute();
            }

            Assert.AreEqual(1, rowsProcessed);
        }

        [Test]
        public void WillRaiseRowProcessedEventUntilItThrows()
        {
            int rowsProcessed = 0;

            using (OutputFibonacciToDatabase fibonaci = new OutputFibonacciToDatabase(25, Should.Throw))
            {
                fibonaci.OutputOperation.OnRowProcessed += delegate { rowsProcessed++; };
                fibonaci.Execute();

                Assert.AreEqual(fibonaci.ThrowingOperation.RowsAfterWhichToThrow, rowsProcessed);
            }
        }

        [Test]
        public void WillRaiseFinishedProcessingEventOnce()
        {
            int finished = 0;

            using (OutputFibonacciToDatabase fibonaci = new OutputFibonacciToDatabase(1, Should.WorkFine))
            {
                fibonaci.OutputOperation.OnFinishedProcessing += delegate { finished++; };
                fibonaci.Execute();
            }

            Assert.AreEqual(1, finished);
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
