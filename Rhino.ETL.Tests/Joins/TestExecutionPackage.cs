using Rhino.ETL.Engine;

namespace Rhino.ETL.Tests.Joins
{
	public class TestExecutionPackage : ExecutionPackage
	{
		public override void RegisterForExecution(Target target,Command action)
		{
			action();
		}
	}
}