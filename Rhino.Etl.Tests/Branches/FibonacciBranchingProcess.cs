using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;
using Rhino.Etl.Core.Pipelines;
using Rhino.Etl.Tests.Fibonacci;
using Rhino.Etl.Tests.Fibonacci.Bulk;

namespace Rhino.Etl.Tests.Branches
{
    public class FibonacciBranchingProcess : EtlProcess
    {
        private readonly int numberOfFibonacciIterations;
        private readonly int numberOfChildOperations;

        public FibonacciBranchingProcess(int numberOfFibonacciIterations, int numberOfChildOperations)
        {
            this.numberOfFibonacciIterations = numberOfFibonacciIterations;
            this.numberOfChildOperations = numberOfChildOperations;
        }

        protected override void Initialize()
        {
            PipelineExecuter = new SingleThreadedPipelineExecuter();

            Register(new FibonacciOperation(numberOfFibonacciIterations));

            var split = new BranchingOperation();

            for (int i = 0; i < numberOfChildOperations; i++)
                split.Add(new FibonacciBulkInsert());

            Register(split);
        }
    }
}