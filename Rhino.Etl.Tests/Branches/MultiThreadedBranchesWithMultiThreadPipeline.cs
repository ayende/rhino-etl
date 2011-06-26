using Rhino.Etl.Core;

namespace Rhino.Etl.Tests.Branches
{
    public class MultiThreadedBranchesWithMultiThreadPipeline : BranchesFixture
    {
        protected override EtlProcess CreateBranchingProcess(int iterations, int childOperations)
        {
            return new MultiThreadedWithMultiThreadPipelineFibonacciBranchingProcess(iterations, childOperations);
        }
    }
}