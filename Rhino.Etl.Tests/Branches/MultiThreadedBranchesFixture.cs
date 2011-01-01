using Rhino.Etl.Core;

namespace Rhino.Etl.Tests.Branches
{
	public class MultiThreadedBranchesFixture : BranchesFixture
	{
		protected override EtlProcess CreateBranchingProcess(int numberOfFibonacciIterations, int numberOfChildOperations)
		{
			return new MultiThreadedFibonacciBranchingProcess(numberOfFibonacciIterations, numberOfChildOperations);
		}
	}
}