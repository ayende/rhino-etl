using System;
using System.Data;
using Xunit;
using Rhino.Etl.Core.Infrastructure;

namespace Rhino.Etl.Tests.Branches
{
    
    public class BranchesFixture : BaseFibonacciTest
    {
        [Fact]
        public void CanBranchThePipeline()
        {
            using (var process = new FibonacciBranchingProcess(30, 2))
                process.Execute();

            AssertCountForFibonacci(60);
        }

        [Fact] 
        public void CanBranchThePipelineEfficiently()
        {
            var initialMemory = GC.GetTotalMemory(true);

            using (var process = new FibonacciBranchingProcess(30000, 10))
                process.Execute();

            var finalMemory = GC.GetTotalMemory(true);

            Assert.True(finalMemory - initialMemory < 10 * 1000 * 1000, "Consuming too much memory");
            AssertCountForFibonacci(300000);
        }

        protected static void AssertCountForFibonacci(int numberOfRows)
        {
            int max = Use.Transaction("test", delegate(IDbCommand cmd)
            {
                cmd.CommandText = "SELECT count(*) FROM Fibonacci";
                return (int) cmd.ExecuteScalar();
            });
            Assert.Equal(numberOfRows, max);
        }
    }
}