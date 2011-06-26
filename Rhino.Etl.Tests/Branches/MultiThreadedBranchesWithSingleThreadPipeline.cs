using Rhino.Etl.Core;

namespace Rhino.Etl.Tests.Branches
{
    public class MultiThreadedBranchesWithSingleThreadPipeline : BranchesFixture
    {
        protected override EtlProcess CreateBranchingProcess(int iterations, int childOperations)
        {
            return new MultiThreadedWithSingleThreadPipelineFibonacciBranchingProcess(iterations, childOperations);
        }
    }
}