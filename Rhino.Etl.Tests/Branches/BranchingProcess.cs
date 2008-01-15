namespace Rhino.Etl.Tests.Branches
{
	using Core;
	using Fibonacci;
	using Rhino.Etl.Core.Operations;
	using Rhino.Etl.Core.Pipelines;

	public class BranchingProcess : EtlProcess
	{
		protected override void Initialize()
		{
			PipelineExecuter = new SingleThreadedPipelineExecuter();

			Register(new FibonacciOperation(30));
			
			BranchingOperation split = new BranchingOperation()
				.Add(Partial
				     	.Register(new MultiplyByThreeOperation())
				     	.Register(new Fibonacci.Bulk.FibonacciBulkInsert()))
				.Add(Partial
				     	.Register(new Fibonacci.Bulk.FibonacciBulkInsert()));

			Register(split);
		}
	}
}