using System;
using System.Collections;
using System.Collections.Generic;
using MbUnit.Framework;
using Rhino.ETL.Tests.Integration;

namespace Rhino.ETL.Tests.Joins
{
	using Engine;
	using Retlang;

	[TestFixture]
	public class Joins : BaseTest
	{
		private EtlConfigurationContext configurationContext;
		private ExecutionPackage package;

		[SetUp]
		public void TestInitialize()
		{
			Pipeline.Current = null;
			configurationContext = BuildContext(@"Joins\join_two_tables.retl");
			package = configurationContext.BuildPackage();
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
			using (configurationContext.EnterContext())
			using (package.EnterContext())
			{
				int rowCount = 0;
				Join join = configurationContext.Joins["JoinUsersAndOrganization"];
				IProcessContextFactory factory = new ProcessContextFactory();
				configurationContext.Pipelines["CopyUsers"].EnterContext();
				factory.Start();
				IProcessContext context = factory.CreateAndStart();
				context.Subscribe<Row>(new TopicEquals("JoinUsersAndOrganization.Output"), delegate(IMessageHeader header, Row row)
				{
					rowCount++;
					Assert.AreEqual(1, row["Id"]);
					Assert.AreEqual(2, row["OrgId"]);
					Assert.AreEqual(2, row["Organization"]);
					Assert.AreEqual("New York", row["City"]);
					Assert.IsNull(row["UserId"]);
					context.Stop();
				});
				context = factory.CreateAndStart();

				Row left = new Row();
				left["Id"] = 1;
				left["OrgId"] = 2;
				left["City"] = "New York";
				Row right = new Row();
				right["UserId"] = 1;
				right["organization id"] = 2;

				join.OutputName = "Output";
				join.Start(context, "Left", "Right");

				context.Publish("Left", left);
				context.Publish("Right", right);

				right = right.Clone();
				right["organization id"] = 3;

				context.Publish("Right", right);

				context.Publish("Left.IsDone", "");
				context.Publish("Right.IsDone", "");

				context.Join();

				Assert.AreEqual(1, rowCount);

				Pipeline.Current = null;
				factory.Stop();
			}

		}

		[Test]
		public void UsingDistinct()
		{
			using (configurationContext.EnterContext())
			using (package.EnterContext())
			{
				int rowCount = 0;
				Pipeline pipeline = configurationContext.Pipelines["CopyUsers"];
				using (pipeline.EnterContext())
				{
					configurationContext.Pipelines["CopyUsers"].EnterContext();
					IProcessContextFactory factory = new ProcessContextFactory();
					factory.Start();
					IProcessContext context = factory.CreateAndStart();
					context.Subscribe<Row>(new TopicEquals("Distinct.Output"),
					                       delegate { rowCount++; });
					IProcessContext context2 = factory.CreateAndStart();

					Transform transform = configurationContext.Transforms["Distinct"];
					Row row = new Row();
					row["Id"] = 1;
					row["OrgId"] = 2;
					row["City"] = "New York";

					transform.Start(context2, "test");

					for (int i = 0; i < 15; i++)
					{
						context2.Publish("test", row);
					}
					context2.Publish("test" + Messages.Done, "");
					context2.Join();
					context.Stop();
					factory.Stop();
				}

				Assert.AreEqual(1, rowCount);
			}
		}


		[Test]
		[Ignore("Not sure how to fix this")]
		public void UsingDistinct_WithParameters_JustOnId()
		{
			using (configurationContext.EnterContext())
			using (package.EnterContext())
			{
				int rowCount = 0;
				Pipeline pipeline = configurationContext.Pipelines["CopyUsers"];
				using (pipeline.EnterContext())
				{
					IProcessContextFactory factory = new ProcessContextFactory();
					factory.Start();
					IProcessContext context = factory.CreateAndStart();
					context.Subscribe<Row>(new TopicEquals("Distinct.Output"),
					                       delegate { rowCount++; });
					IProcessContext context2 = factory.CreateAndStart();

					Transform transform = configurationContext.Transforms["Distinct"];

					transform.Start(context2, "test");
					Hashtable hashtable = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
					hashtable["Columns"] = new string[] {"Id"};
					Assert.Fail("Where do I put the table?");
					Row row = new Row();
					row["Id"] = 1;
					row["OrgId"] = 2;
					row["City"] = "New York";

					context2.Publish("test", row);

					row["Id"] = 1;
					row["OrgId"] = 2;
					row["City"] = "Tel Aviv";
					context2.Publish("test", row);

					context2.Publish("test.IsDone", "");

					context2.Join();
					context.Stop();
					factory.Stop();
				}
				Assert.AreEqual(1, rowCount);
			}
		}
		[Test]
		public void RowCount()
		{
			using (configurationContext.EnterContext())
			using (package.EnterContext())
			{
				int rowCount = 0;
				Pipeline pipeline = configurationContext.Pipelines["CopyUsers"];
				using (pipeline.EnterContext())
				{
					IProcessContextFactory factory = new ProcessContextFactory();
					factory.Start();
					IProcessContext context = factory.CreateAndStart();
					context.Subscribe<Row>(new TopicEquals("CountRows.Output"),
					                       delegate(IMessageHeader header, Row msg)
					                       {
					                       	object rowCountObj = msg["RowCount"];
					                       	rowCount = (int) rowCountObj;
					                       });
					IProcessContext context2 = factory.CreateAndStart();


					Transform transform = configurationContext.Transforms["CountRows"];
					transform.Start(context2, "test");
					Row row = new Row();

					for (int i = 0; i < 14; i++)
					{
						context2.Publish("test", row);
					}
					context2.Publish("test" + Messages.Done, "");

					context2.Join();
					context.Stop();
					factory.Stop();
				}

				Assert.AreEqual(14, rowCount);
			}
		}

		[Test]
		public void SumUsingTransform()
		{
			using (configurationContext.EnterContext())
			using (package.EnterContext())
			{
				int idSum = 0;
				int salarySum = 0;
				Pipeline pipeline = configurationContext.Pipelines["CopyUsers"];
				using (pipeline.EnterContext())
				{
					IProcessContextFactory factory = new ProcessContextFactory();
					factory.Start();
					IProcessContext context = factory.CreateAndStart();
					context.Subscribe<Row>(new TopicEquals("CalcSumOfSalaryAndId.Output"),
					                       delegate(IMessageHeader header, Row processedRow)
					                       {
					                       	idSum = (int) processedRow["IdSum"];
					                       	salarySum = (int) processedRow["SalarySum"];
					                       });
					IProcessContext context2 = factory.CreateAndStart();


					Transform transform = configurationContext.Transforms["CalcSumOfSalaryAndId"];
					transform.Start(context2, "test");
					Row row = new Row();
					row["id"] = 2;
					row["salary"] = 10;
					for (int i = 0; i < 14; i++)
					{
						context2.Publish("test", row);
					}

					context2.Publish("test" + Messages.Done, "");

					context2.Join();
					context.Stop();
					factory.Stop();
				}

				Assert.AreEqual(28, idSum);
				Assert.AreEqual(140, salarySum);
			}
		}
	}
}