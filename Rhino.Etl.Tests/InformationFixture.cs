namespace Rhino.Etl.Tests
{
    using Fibonacci;
    using MbUnit.Framework;

    [TestFixture]
    public class InformationFixture 
    {
        [Test]
        public void WillReportRowProcessedUsage()
        {
            InMemoryFibonacci fibonacci = new InMemoryFibonacci();
            fibonacci.Execute();
            Assert.AreEqual(25, fibonacci.FibonacciOperation.Statistics.OutputtedRows);
        }

        [Test]
        public void WillReportWhenOpeartionEnded()
        {
            bool finished = false;
            InMemoryFibonacci fibonacci = new InMemoryFibonacci();
            fibonacci.FibonacciOperation.OnFinishedProcessing += delegate
            {
                finished = true;
            };
            fibonacci.Execute();
            Assert.IsTrue(finished);
        }
    }
}