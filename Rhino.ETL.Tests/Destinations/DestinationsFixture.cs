using MbUnit.Framework;

namespace Rhino.ETL.Tests.Destinations
{
	using Engine;

	[TestFixture]
	public class DestinationsFixture : BaseTest
	{
		private EtlConfigurationContext configurationContext;

		[SetUp]
		public void TestInitialize()
		{
			configurationContext = BuildContext(@"Destinations\destinations_only.retl");
		}

		[Test]
		public void Destination_BatchSize_CanBeSpecifiedFromDSL()
		{
			DataDestination dest = configurationContext.Destinations["SouthSand"];
			Assert.AreEqual(500, dest.BatchSize );
		}
	}
}
