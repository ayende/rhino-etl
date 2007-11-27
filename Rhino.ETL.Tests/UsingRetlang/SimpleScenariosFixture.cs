namespace Rhino.ETL2.Tests.UsingRetlang
{
	using ETL;
	using MbUnit.Framework;
	using Retlang;
	using Rhino.ETL.Engine;

	[TestFixture]
	public class SimpleScenariosFixture
	{
		[Test]
		public void CanPassDataFromSourceToDestination()
		{
			EtlConfigurationContext ecc = new FakeEtlConfigurationContext();
			using (ecc.EnterContext())
			{
				Pipeline pipeline = new Pipeline("foo");
				using (pipeline.EnterContext())
				{
					IProcessContextFactory factory = new ProcessContextFactory();
					factory.Start();
					IProcessContext producer = factory.Create();
					producer.Start();
					IProcessContext consumer = factory.Create();
					consumer.Start();

					DataDestination dd = new DataDestination("test");
					PutInSyncList putInSyncList = new PutInSyncList();
					dd.OnRowBlock = putInSyncList;
					dd.Start(consumer, "test.Output");

					DataSource ds = new DataSource("test");
					ds.Execute(new IntGenerator(ds));
					ds.Start(producer, "Output");

					producer.Join();
					consumer.Join();
					factory.Stop();
					factory.Join();

					Assert.AreEqual(50, putInSyncList.List.Count);
					for (int i = 0; i < 50; i++)
					{
						Assert.AreEqual(i, putInSyncList.List[i]);
					}
				}
			}
		}

		[Test]
		public void CanJoinDataFromMultiplySourcesToSingleDestination()
		{
			EtlConfigurationContext ecc = new FakeEtlConfigurationContext();
			using (ecc.EnterContext())
			{
				Pipeline pipeline = new Pipeline("foo");

				using (pipeline.EnterContext())
				{
					IProcessContextFactory factory = new ProcessContextFactory();
					factory.Start();
					IProcessContext producer1 = factory.Create();
					producer1.Start();
					IProcessContext producer2 = factory.Create();
					producer2.Start();
					IProcessContext consumer = factory.Create();
					consumer.Start();
					IProcessContext joinConsumer = factory.Create();
					joinConsumer.Start();

					Join join = new FakeJoin("test join");
					join.Condition =
						new DelegateCallable(
							delegate(object[] args) { return (int) ((Row) args[0])["id"] == (int) ((Row) args[1])["id"]; });
					join.Start(joinConsumer, "test1.Left", "test2.Right");

					DataDestination dd = new DataDestination("test");
					PutInSyncList putInSyncList = new PutInSyncList();
					dd.OnRowBlock = putInSyncList;
					dd.Start(consumer, "test join.Output");

					DataSource ds = new DataSource("test1");
					ds.OutputName = "Left";
					ds.Execute(new IntGenerator(ds));
					ds.Start(producer1, "Output");

					DataSource ds2 = new DataSource("test2");
					ds2.OutputName = "Right";
					ds2.Execute(new IntGenerator(ds2));
					ds2.Start(producer2, "Output");

					producer1.Join();
					joinConsumer.Join();
					consumer.Join();
					factory.Stop();
					factory.Join();

					Assert.AreEqual(50, putInSyncList.List.Count);
					for (int i = 0; i < 50; i++)
					{
						Assert.AreEqual(i*i, putInSyncList.List[i]);
					}
				}
			}
		}

		[Test]
		public void CanUseTranforms()
		{
			EtlConfigurationContext ecc = new FakeEtlConfigurationContext();
			using (ecc.EnterContext())
			{
				Pipeline pipeline = new Pipeline("foo");

				using (pipeline.EnterContext())
				{
					IProcessContextFactory factory = new ProcessContextFactory();
					factory.Start();
					IProcessContext producer = factory.Create();
					producer.Start();
					IProcessContext transformer = factory.Create();
					transformer.Start();
					IProcessContext consumer = factory.Create();
					consumer.Start();

					DataDestination dd = new DataDestination("test");
					PutInSyncList putInSyncList = new PutInSyncList();
					dd.OnRowBlock = putInSyncList;
					dd.Start(consumer, "foo.Output");

					Transform transform = new FakeTransform("foo");
					transform.OutputName = "Output";
					transform.Start(transformer, "test.OutputToTransform");

					DataSource ds = new DataSource("test");
					ds.Execute(new IntGenerator(ds));
					ds.OutputName = "OutputToTransform";
					ds.Start(producer, "");

					producer.Join();
					transformer.Join();
					consumer.Join();
					factory.Stop();
					factory.Join();

					Assert.AreEqual(50, putInSyncList.List.Count);
					for (int i = 0; i < 50; i++)
					{
						Assert.AreEqual("Id: " + i, putInSyncList.List[i]);
					}
				}
			}
		}
	}
}