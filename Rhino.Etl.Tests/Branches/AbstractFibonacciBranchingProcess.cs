using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;
using Rhino.Etl.Tests.Fibonacci;
using Rhino.Etl.Tests.Fibonacci.Bulk;

namespace Rhino.Etl.Tests.Branches
{
    public abstract class AbstractFibonacciBranchingProcess : EtlProcess
    {
        private readonly int numberOfFibonacciIterations;
        private readonly int numberOfChildOperations;

        protected AbstractFibonacciBranchingProcess(int numberOfFibonacciIterations, int numberOfChildOperations)
        {
            this.numberOfFibonacciIterations = numberOfFibonacciIterations;
            this.numberOfChildOperations = numberOfChildOperations;
        }

        protected override void Initialize()
        {
            PipelineExecuter = CreatePipelineExecuter();

            Register(new FibonacciOperation(numberOfFibonacciIterations));

            var split = CreateBranchingOperation();

            for (int i = 0; i < numberOfChildOperations; i++)
                split.Add(new FibonacciBulkInsert());

            Register(split);
        }

        protected abstract AbstractBranchingOperation CreateBranchingOperation();

        protected abstract IPipelineExecuter CreatePipelineExecuter();
    }
}