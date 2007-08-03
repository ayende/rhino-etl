namespace Rhino.ETL.Tests.Joins
{
	public class TestExecutionPackage : ExecutionPackage
	{
		public override void RegisterForExecution(Command action)
		{
			action();
		}
	}
}