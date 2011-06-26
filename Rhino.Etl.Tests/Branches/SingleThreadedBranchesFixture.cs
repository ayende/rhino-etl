using Rhino.Etl.Core;

namespace Rhino.Etl.Tests.Branches
{
    public class SingleThreadedBranchesFixture : BranchesFixture
    {
        protected override EtlProcess CreateBranchingProcess(int iterations, int childOperations)
        {
            return new SingleThreadedFibonacciBranchingProcess(iterations, childOperations);
        }
    }
}