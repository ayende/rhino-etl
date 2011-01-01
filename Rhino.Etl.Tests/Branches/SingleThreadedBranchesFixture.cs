using Rhino.Etl.Core;

namespace Rhino.Etl.Tests.Branches
{
	public class SingleThreadedBranchesFixture : BranchesFixture
	{
		protected override EtlProcess CreateBranchingProcess(int numberOfFibonacciIterations, int numberOfChildOperations)
		{
			return new SingleThreadedFibonacciBranchingProcess(numberOfFibonacciIterations, numberOfChildOperations);
		}
	}
}