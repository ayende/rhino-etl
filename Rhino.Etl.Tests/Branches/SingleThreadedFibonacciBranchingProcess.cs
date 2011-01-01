using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;
using Rhino.Etl.Core.Pipelines;

namespace Rhino.Etl.Tests.Branches
{
	public class SingleThreadedFibonacciBranchingProcess : AbstractFibonacciBranchingProcess
	{
		public SingleThreadedFibonacciBranchingProcess(int numberOfFibonacciIterations, int numberOfChildOperations)
			: base(numberOfFibonacciIterations, numberOfChildOperations)
		{
		}

		protected override AbstractBranchingOperation CreateBranchingOperation()
		{
			return new BranchingOperation();
		}

		protected override IPipelineExecuter CreatePipelineExecuter()
		{
			return new SingleThreadedPipelineExecuter();
		}
	}
}