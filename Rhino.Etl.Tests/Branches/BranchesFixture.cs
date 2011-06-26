using System;
using Rhino.Etl.Core;
using Xunit;
using Rhino.Etl.Core.Infrastructure;

namespace Rhino.Etl.Tests.Branches
{
    public abstract class BranchesFixture : BaseFibonacciTest
    {
        [Fact]
        public void CanBranchThePipeline()
        {
            using (var process = CreateBranchingProcess(30, 2))
                process.Execute();

            AssertFibonacci(30, 2);
        }

        [Fact] 
        public void CanBranchThePipelineEfficiently()
        {
            const int iterations = 30000;
            const int childOperations = 10;

            var initialMemory = GC.GetTotalMemory(true);

            using (var process = CreateBranchingProcess(iterations, childOperations))
                process.Execute();

            var finalMemory = GC.GetTotalMemory(true);
            var consumedMemory = finalMemory - initialMemory;
            var tooMuchMemory = Math.Pow(2, 20);
            
            Assert.True(consumedMemory < tooMuchMemory, "Consuming too much memory - (" + consumedMemory.ToString() + " >= " + tooMuchMemory + ")");
            AssertFibonacci(iterations, childOperations);
        }

        protected abstract EtlProcess CreateBranchingProcess(int iterations, int childOperations);

        protected static void AssertFibonacci(int iterations, int repetitionsPerIteration)
        {
            AssertTotalItems(iterations * repetitionsPerIteration);

            AssertRepetitions(repetitionsPerIteration);
        }

        private static void AssertRepetitions(int repetitionsPerIteration)
        {
            int wrongRepetitions = Use.Transaction("test", cmd =>
            {
                cmd.CommandText =
string.Format(@"    SELECT count(*) 
    FROM (
        SELECT id, count(*) as count
        FROM Fibonacci
        GROUP BY id
        HAVING count(*) <> {0}
    ) as ignored", repetitionsPerIteration);
                return (int)cmd.ExecuteScalar();
            });

            Assert.Equal(1 /* 1 is repetated twice the others */, wrongRepetitions);
        }

        private static void AssertTotalItems(int expectedCount)
        {
            int totalCount = Use.Transaction("test", cmd =>
                                                     {
                                                         cmd.CommandText = "SELECT count(*) FROM Fibonacci";
                                                         return (int) cmd.ExecuteScalar();
                                                     });
            
            Assert.Equal(expectedCount, totalCount);
        }
    }
}