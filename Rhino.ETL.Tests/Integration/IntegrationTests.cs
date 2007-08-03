using MbUnit.Framework;

namespace Rhino.ETL.Tests.Integration
{
	[TestFixture]
	public class IntegrationTests : IntegrationTestBase
	{
		[Test]
		public void WillCopyRowsFromOneTableToAnother()
		{
			ExecutePackage("basic");
			AssertRowNotZero();
		}

		[Test]
		public void WillSkipSingleRowWithBadEmail()
		{
			ExecutePackage("basic");
			AssertRowCount(8);
		}


		[Test]
		public void CopyUsersWithJoin()
		{
			EtlConfigurationContext configurationContext = BuildContext(@"Joins\join_two_tables.retl"); 
			ExecutionPackage package = configurationContext.BuildPackage();
			Pipeline pipeline = configurationContext.Pipelines["CopyUsers"];
			package.Execute(pipeline);

			AssertRowCount(4);
		}

		private static void ExecutePackage(string name)
		{
			EtlConfigurationContext configurationContext = BuildContext(@"Integration\" + name + ".retl");
			ExecutionPackage package = configurationContext.BuildPackage();
			package.Execute();
		}
	}
}
