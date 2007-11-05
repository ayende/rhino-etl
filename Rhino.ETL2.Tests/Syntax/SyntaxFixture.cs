using System.Collections.Generic;
using MbUnit.Framework;

namespace Rhino.ETL.Tests.EndToEnd
{
	using Engine;

	[TestFixture]
	public class SyntaxFixture : BaseTest
	{
		private EtlConfigurationContext configurationContext;

		[SetUp]
		public void TestInitialize()
		{
			configurationContext = BuildContext(@"Syntax\full_package.retl");
		}
		
		[Test]
		public void ModelPassesValidation()
		{
			Assert.IsTrue(configurationContext.Validate());
		}

		[Test]
		public void SecondStagePassWillPass()
		{
			configurationContext.BuildPackage();
		}

		[Test]
		public void SecondStagePassWillConnectObjects()
		{
			configurationContext.BuildPackage();
			foreach (DataSource value in configurationContext.Sources.Values)
			{
				Assert.IsNotNull(value.Connection);
			}
			foreach (DataDestination value in configurationContext.Destinations.Values)
			{
				Assert.IsNotNull(value.Connection);
			}
			foreach (Pipeline value in configurationContext.Pipelines.Values)
			{
				foreach (PipelineAssociation association in value.Associations)
				{
					Assert.IsNotNull(association.Input);
					Assert.IsNotNull(association.Output);
				}
			}
		}
	}
}
