using System.Collections;
using System.Data;
using MbUnit.Framework;

namespace Rhino.ETL.Tests.Pipelines
{
	using Engine;

	[TestFixture]
	public class PipelineFixture : BaseTest
	{
		private EtlConfigurationContext configurationContext;
		private Pipeline pipeline;

		[SetUp]
		public void TestInitialize()
		{
			configurationContext = BuildContext(@"Pipelines\Pipeline.retl");
			pipeline = configurationContext.Pipelines["CopyFromNorthwindToSouthSand"];
		}

		[Test]
		public void Pipeline_Structure_CanBeSpecifiedInDSL()
		{
			Assert.AreEqual(3, pipeline.Associations.Count);
		}

		[Test]
		public void CanSpecifyAssociationTypeInDSL()
		{
			Assert.AreEqual(AssociationType.Sources, pipeline.Associations[0].FromType);
			Assert.AreEqual(AssociationType.Transforms, pipeline.Associations[2].ToType);
		}

		[Test]
		public void CanSpecifySourceFromToInDSL()
		{
			Assert.AreEqual("NorthwindSource", pipeline.Associations[0].From);
			Assert.AreEqual("RemoveRowsWithoutRegion", pipeline.Associations[0].To);
			Assert.AreEqual("RemoveRowsWithoutRegion", pipeline.Associations[1].From);
			Assert.AreEqual("SouthSandDestination", pipeline.Associations[1].To);
			Assert.AreEqual("RemoveRowsWithoutRegion", pipeline.Associations[2].From);
			Assert.AreEqual("LogRowId", pipeline.Associations[2].To);
		}

		[Test]
		public void CanSpecifyInputQueueInDSL()
		{
			Assert.AreEqual("RegionlessRows", pipeline.Associations[2].FromQueue);
			Assert.AreEqual("ExplicitInput", pipeline.Associations[2].ToQueue);
		}

		[Test]
		public void CanSpecifyParametersInDSL()
		{
			IList colums = (IList)pipeline.Associations[0].Parameters["Columns"];
			Assert.AreEqual("Col1", colums[0] );
			Assert.AreEqual("Col2", colums[1]);
		}
	}
}