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
			package.Execute("default");

			AssertRowCount(4);
		}

		[Test]
		public void When_NOT_UsingTransaction_AndExceptionThrown_WillKeepCompletedOperation()
		{
			EtlConfigurationContext context = BuildContext(@"Errors\target_with_throwing_component.retl");

			ExecutionPackage package = context.BuildPackage();
			ExecutionResult result = package.Execute("withOutTransaction");
			Assert.AreEqual(ExecutionStatus.Failure, result.Status);

			AssertRowCount(8);
		}

		[Test]
		public void WhenUsingTransaction_AndExceptionThrown_WillRollBack()
		{
			EtlConfigurationContext context = BuildContext(@"Errors\target_with_throwing_component.retl");

			ExecutionPackage package = context.BuildPackage();
			ExecutionResult result = package.Execute("withTransaction");
			Assert.AreEqual(ExecutionStatus.Failure, result.Status);

			AssertRowCount(0);
		}

		[Test]
		public void WhenUsingTransaction_WithExplicitIsolationLevel_AndExceptionThrown_WillRollBack()
		{
			EtlConfigurationContext context = BuildContext(@"Errors\target_with_throwing_component.retl");

			ExecutionPackage package = context.BuildPackage();
			ExecutionResult result = package.Execute("transactionWithIsolationLevel");
			Assert.AreEqual(ExecutionStatus.Failure, result.Status);

			AssertRowCount(0);
		}

		private static void ExecutePackage(string name)
		{
			EtlConfigurationContext configurationContext = BuildContext(@"Integration\" + name + ".retl");
			ExecutionPackage package = configurationContext.BuildPackage();
			package.Execute("default");
		}
	}
}
