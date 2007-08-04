using MbUnit.Framework;
using Rhino.ETL.Commands;
using Rhino.ETL.Engine;

namespace Rhino.ETL.Tests.Targets
{
	[TestFixture]
	public class TargetFixture : BaseTest
	{
		private EtlConfigurationContext configurationContext;

		[SetUp]
		public void TestInitialize()
		{
			configurationContext = BuildContext(@"Targets\targets.retl");
		}
		
		[Test]
		public void CanSpecifyParallelExecution()
		{
			Target target = configurationContext.Targets["default"];
			using(configurationContext.EnterContext())
				target.Prepare();
			ExecuteInParallelCommand command = (ExecuteInParallelCommand)target.Commands[0];
			Assert.IsNotNull(command);
			Assert.AreEqual(2, command.Commands.Count);
		}

		[Test]
		public void CanSpecifySequenceExecution()
		{
			Target target = configurationContext.Targets["default"];
			using (configurationContext.EnterContext())
				target.Prepare();
			ExecuteInSequenceCommand command = (ExecuteInSequenceCommand)target.Commands[1];
			Assert.IsNotNull(command);
			Assert.AreEqual(2, command.Commands.Count);
		}
	}
}
