namespace Rhino.Etl.Tests
{
    using System;
    using System.Collections.Generic;
    using Xunit;
    using Fibonacci.Output;

    
    public class OutputCommandFixture : BaseFibonacciTest
    {
        [Fact]
        public void CanInsertToDatabaseFromInMemoryCollection()
        {
            OutputFibonacciToDatabase fibonaci = new OutputFibonacciToDatabase(25, Should.WorkFine);
            fibonaci.Execute();

            Assert25ThFibonacci();
        }

        [Fact]
        public void WillRaiseRowProcessedEvent()
        {
            int rowsProcessed = 0;

            using (OutputFibonacciToDatabase fibonaci = new OutputFibonacciToDatabase(1, Should.WorkFine))
            {
                fibonaci.OutputOperation.OnRowProcessed += delegate { rowsProcessed++; };
                fibonaci.Execute();
            }

            Assert.Equal(1, rowsProcessed);
        }

        [Fact]
        public void WillRaiseRowProcessedEventUntilItThrows()
        {
            int rowsProcessed = 0;

            using (OutputFibonacciToDatabase fibonaci = new OutputFibonacciToDatabase(25, Should.Throw))
            {
                fibonaci.OutputOperation.OnRowProcessed += delegate { rowsProcessed++; };
                fibonaci.Execute();

                Assert.Equal(fibonaci.ThrowingOperation.RowsAfterWhichToThrow, rowsProcessed);
            }
        }

        [Fact]
        public void WillRaiseFinishedProcessingEventOnce()
        {
            int finished = 0;

            using (OutputFibonacciToDatabase fibonaci = new OutputFibonacciToDatabase(1, Should.WorkFine))
            {
                fibonaci.OutputOperation.OnFinishedProcessing += delegate { finished++; };
                fibonaci.Execute();
            }

            Assert.Equal(1, finished);
        }

        [Fact]
        public void WhenErrorIsThrownWillRollbackTransaction()
        {
            OutputFibonacciToDatabase fibonaci = new OutputFibonacciToDatabase(25, Should.Throw);
            fibonaci.Execute();
            Assert.Equal(1, new List<Exception>(fibonaci.GetAllErrors()).Count);
            AssertFibonacciTableEmpty();
        }
    }
}
