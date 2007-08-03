using System.Collections.Generic;
using MbUnit.Framework;

namespace Rhino.ETL.Tests.Joins
{
	[TestFixture]
	public class Joins : BaseTest
	{
		private EtlConfigurationContext configurationContext;

		[SetUp]
		public void TestInitialize()
		{
			configurationContext = BuildContext(@"Joins\join_two_tables.retl");
			ExecutionPackage.Current = new TestExecutionPackage();
		}

		[TearDown]
		public void TestCleanup()
		{
			ExecutionPackage.Current = null;
		}


		[Test]
		public void CanSepcifyJoinInDSL()
		{
			Join join = configurationContext.Joins["JoinUsersAndOrganization"];
			Assert.IsNotNull(join);
		}

		[Test]
		public void SimpleEqualityJoin()
		{
			int rowCount = 0;
			Join join = configurationContext.Joins["JoinUsersAndOrganization"];
			TestPipeLineStage pipeLineStage = TestOutput.GetPipelineStage();
			pipeLineStage.OnProcess("Output", delegate(Row row)
			{
				rowCount++;
				Assert.AreEqual(1, row["Id"]);
				Assert.AreEqual(2, row["OrgId"]);
				Assert.AreEqual(2, row["Organization"]);
				Assert.AreEqual("New York", row["City"]);
				Assert.IsNull(row["UserId"]);
			});

			join.RegisterForwarding(pipeLineStage);

			Row left = new Row();
			left["Id"] = 1;
			left["OrgId"] = 2;
			left["City"] = "New York";
			Row right = new Row();
			right["UserId"] = 1;
			right["organization id"] = 2;

			join.Process("Left", left, null);
			join.Process("Right", right, null);
			join.Complete("Left");
			join.Complete("Right");

			Assert.AreEqual(1, rowCount);
		}

		[Test]
		[Ignore("not implemented")]
		public void NonEquialityJoin()
		{
		}

		[Test]
		[Ignore("not implemented")]
		public void JoinUsingOr()
		{
		}

		[Test]
		[Ignore("not implemented")]
		public void JoinLeftToLeft()
		{
		}

		[Test]
		[Ignore("not implemented")]
		public void JoinRightToRight()
		{
		}

		[Test]
		[Ignore("not implemented")]
		public void LeftJoin()
		{
		}

		[Test]
		[Ignore("not implemented")]
		public void RightJoin()
		{
		}

		[Test]
		[Ignore("not implemented")]
		public void FullJoin()
		{
		}

		[Test]
		[Ignore("not implemented")]
		public void OnClauseThrows()
		{
		}

		[Test]
		[Ignore("not implemented")]
		public void TransformationClauseThrows()
		{
		}
	}
}