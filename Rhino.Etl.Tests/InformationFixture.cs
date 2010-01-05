namespace Rhino.Etl.Tests
{
    using Fibonacci;
    using Xunit;

    
    public class InformationFixture 
    {
        [Fact]
        public void WillReportRowProcessedUsage()
        {
            InMemoryFibonacci fibonacci = new InMemoryFibonacci();
            fibonacci.Execute();
            Assert.Equal(25, fibonacci.FibonacciOperation.Statistics.OutputtedRows);
        }

        [Fact]
        public void WillReportWhenOpeartionEnded()
        {
            bool finished = false;
            InMemoryFibonacci fibonacci = new InMemoryFibonacci();
            fibonacci.FibonacciOperation.OnFinishedProcessing += delegate
            {
                finished = true;
            };
            fibonacci.Execute();
            Assert.True(finished);
        }
    }
}