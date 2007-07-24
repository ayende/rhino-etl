using System.Collections.Generic;
using MbUnit.Framework;

namespace Rhino.ETL.Tests.EndToEnd
{
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
				Assert.IsNotNull(value.ConnectionInstance);
			}
			foreach (DataDestination value in configurationContext.Destinations.Values)
			{
				Assert.IsNotNull(value.ConnectionInstance);
			}
			foreach (Pipeline value in configurationContext.Pipelines.Values)
			{
				foreach (PipelineAssociation association in value.Associations)
				{
					Assert.IsNotNull(association.FromInstance);
					Assert.IsNotNull(association.ToInstance);
				}
			}
		}
	}
}
