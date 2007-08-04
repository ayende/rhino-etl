using System;
using System.Collections;
using System.Collections.Generic;
using MbUnit.Framework;
using Rhino.ETL.Tests.Integration;

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
			new TestExecutionPackage().EnterContext();
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
			TestPipeLineStage pipeLineStage = TestOutput.GetPipelineStage(
				configurationContext.Pipelines["CopyUsers"]
				);
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
			right = right.Clone();
			right["organization id"] = 3;
			join.Process("Right", right, null);
			join.Complete("Left");
			join.Complete("Right");

			Assert.AreEqual(1, rowCount);
		}


		[Test]
		public void UsingDistinct()
		{
			int rowCount = 0;
			Pipeline pipeline = configurationContext.Pipelines["CopyUsers"];
			using (pipeline.EnterContext())
			{
				TestPipeLineStage pipeLineStage = TestOutput.GetPipelineStage(
					pipeline
					);
				pipeLineStage.OnProcess("Output", delegate { rowCount++; });


				Transform transform = configurationContext.Transforms["Distinct"];
				transform.RegisterForwarding(pipeLineStage);
				Row row = new Row();
				row["Id"] = 1;
				row["OrgId"] = 2;
				row["City"] = "New York";

				for (int i = 0; i < 15; i++)
				{
					transform.Process("Output", row, new Hashtable());
				}
				transform.Complete("Output");
			}
			Assert.AreEqual(1, rowCount);
		}

		[Test]
		public void UsingDistinct_WithParameters_JustOnId()
		{
			int rowCount = 0;
			Pipeline pipeline = configurationContext.Pipelines["CopyUsers"];
			using (pipeline.EnterContext())
			{
				TestPipeLineStage pipeLineStage = TestOutput.GetPipelineStage(
					pipeline
					);
				pipeLineStage.OnProcess("Output", delegate { rowCount++; });


				Transform transform = configurationContext.Transforms["Distinct"];
				transform.RegisterForwarding(pipeLineStage);
				Hashtable hashtable = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
				hashtable["Columns"] = new string[] {"Id"};
				Row row = new Row();
				row["Id"] = 1;
				row["OrgId"] = 2;
				row["City"] = "New York";
				transform.Process("Output", row, hashtable);
				row["Id"] = 1;
				row["OrgId"] = 2;
				row["City"] = "Tel Aviv";
				transform.Process("Output", row, hashtable);

				transform.Complete("Output");
			}
			Assert.AreEqual(1, rowCount);
		}


		[Test]
		public void RowCount()
		{
			int rowCount = 0;
			Pipeline pipeline = configurationContext.Pipelines["CopyUsers"];
			using (pipeline.EnterContext())
			{
				TestPipeLineStage pipeLineStage = TestOutput.GetPipelineStage(
					pipeline
					);
				pipeLineStage.OnProcess("Output", delegate(Row processedRow)
				{
					object rowCountObj = processedRow["RowCount"];
					rowCount = (int)rowCountObj;
				});

				Transform transform = configurationContext.Transforms["CountRows"];
				transform.RegisterForwarding(pipeLineStage);
				Row row = new Row();

				for (int i = 0; i < 14; i++)
				{
					transform.Process("Output", row, null);
				}

				transform.Complete("Output");
			}

			Assert.AreEqual(14, rowCount );
		}

		[Test]
		public void SumUsingTransform()
		{
			int idSum = 0;
			int salarySum = 0;
			Pipeline pipeline = configurationContext.Pipelines["CopyUsers"];
			using (pipeline.EnterContext())
			{
				TestPipeLineStage pipeLineStage = TestOutput.GetPipelineStage(
					pipeline
					);
				pipeLineStage.OnProcess("Output", delegate(Row processedRow)
				{
					idSum += (int)processedRow["IdSum"];
					salarySum += (int)processedRow["SalarySum"];
				});

				Transform transform = configurationContext.Transforms["CalcSumOfSalaryAndId"];
				transform.RegisterForwarding(pipeLineStage);
				Row row = new Row();
				row["id"] = 2;
				row["salary"] = 10;
				for (int i = 0; i < 14; i++)
				{
					transform.Process("Output", row, null);
				}

				transform.Complete("Output");
			}

			Assert.AreEqual(28, idSum);
			Assert.AreEqual(140, salarySum);
		}
	}
}