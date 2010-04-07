namespace Rhino.Etl.Tests.LoadTest
{
	using Core;
	using Rhino.Etl.Core.Pipelines;

	public class Join_250_000_UsersWithMostlyFallingOut : EtlProcess
	{
		public  AccumulateResults operation;

		protected override void Initialize()
		{
			PipelineExecuter = new SingleThreadedPipelineExecuter();

			Register(new JoinUsersAndIds()
			         	.Left(new GenerateUsers(25000))
			         	.Right(new GenerateRandomIds(15000)));
			operation = new AccumulateResults();
			Register(this.operation);
		}
	}
}