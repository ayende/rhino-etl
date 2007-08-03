using MbUnit.Framework;
using Rhino.ETL.Exceptions;

namespace Rhino.ETL.Tests
{
	[TestFixture]
	public class ValidationsFixture : BaseTest
	{
		[Test]
		public void ConnectionDoesNotExists()
		{
			EtlConfigurationContext configurationContext
				= BuildContext(@"Connections\connection_only.retl");
			using (configurationContext.EnterContext())
			{
				DataSource source = new DataSource("test");
				source.Connection = "Doesn't exist";
			}
			Assert.IsFalse(configurationContext.Validate());
			Assert.AreEqual("Could not find connection 'Doesn't exist' in context 'connection_only'",
			                configurationContext.ValidationMessages[0]);
		}

		[Test]
		public void UnknownPipelineAssoications()
		{
			EtlConfigurationContext configurationContext
				= BuildContext(@"Pipelines\Pipeline.retl");
			Assert.IsFalse(configurationContext.Validate());
			Assert.AreEqual(
				"Could not find element 'NorthwindSource' on association #0 in pipeline [CopyFromNorthwindToSouthSand]",
				configurationContext.ValidationMessages[0]);
			Assert.AreEqual(
				"Could not find element 'RemoveRowsWithoutRegion' on association #0 in pipeline [CopyFromNorthwindToSouthSand]",
				configurationContext.ValidationMessages[1]);
			Assert.AreEqual(
				"Could not find element 'RemoveRowsWithoutRegion' on association #1 in pipeline [CopyFromNorthwindToSouthSand]",
				configurationContext.ValidationMessages[2]);
			Assert.AreEqual(
				"Could not find element 'SouthSandDestination' on association #1 in pipeline [CopyFromNorthwindToSouthSand]",
				configurationContext.ValidationMessages[3]);
			Assert.AreEqual(
				"Could not find element 'RemoveRowsWithoutRegion' on association #2 in pipeline [CopyFromNorthwindToSouthSand]",
				configurationContext.ValidationMessages[4]);
			Assert.AreEqual("Could not find element 'LogRowId' on association #2 in pipeline [CopyFromNorthwindToSouthSand]",
			                configurationContext.ValidationMessages[5]);
		}

		[Test]
		public void AmbigiousPipelineAssociation()
		{
			EtlConfigurationContext configurationContext
				= BuildContext(@"Pipelines\Pipeline.retl");
			using (configurationContext.EnterContext())
			{
				DataSource source = new DataSource("SouthSandDestination");
				source.Connection = "Doesn't exist";
				DataDestination destination = new DataDestination("SouthSandDestination");
				destination.Connection = "Doesn't exist";
			}
			Assert.IsFalse(configurationContext.Validate());
			bool containsAmbigousMatchErrorMessage = configurationContext.ValidationMessages
				.Contains(
				"Ambigious match for 'SouthSandDestination' on association #1 in pipeline [CopyFromNorthwindToSouthSand] - you need to qualify it with Sources.SouthSandDestination, Destinations.SouthSandDestination or Transforms.SouthSandDestination or Joins.SouthSandDestination");
			Assert.IsTrue(containsAmbigousMatchErrorMessage);
		}

		[Test]
		public void CanUsePrefixesToAvoidAmbiguty()
		{
			EtlConfigurationContext configurationContext
				= BuildContext(@"Pipelines\Pipeline.retl");
			using (configurationContext.EnterContext())
			{
				DataSource source = new DataSource("NorthwindSource");
				source.Connection = "Doesn't exist";
				DataDestination destination = new DataDestination("NorthwindSource");
				destination.Connection = "Doesn't exist";
			}
			Assert.IsFalse(configurationContext.Validate());
			bool containsAmbigousMatchErrorMessage = configurationContext.ValidationMessages
				.Contains(
				"Ambigious match for 'NorthwindSource' on association #1 in pipeline [CopyFromNorthwindToSouthSand] - you need to qualify it with Sources.SouthSandDestination, Destinations.SouthSandDestination or Transforms.SouthSandDestination or Joins.SouthSandDestination");
			Assert.IsFalse(containsAmbigousMatchErrorMessage);
		}

		[Test]
		[ExpectedException(typeof(TooManyConcurrentConnectionsRequiredException),
		   "Pipeline 'CopyOrders' requires 2 concurrent connections from 'NorthwindConnection', but limit is 1")]
		public void WillThrowExceptionIfNotEnoughConnectionsForPipeline()
		{
			EtlConfigurationContext configurationContext
				= BuildContext(@"Syntax\full_package.retl");
			configurationContext.Connections["NorthwindConnection"].ConcurrentConnections = 1;
			configurationContext.BuildPackage();
		}
	}
}