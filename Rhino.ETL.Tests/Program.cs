namespace Rhino.ETL2.Tests
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using Boo.Lang;
	using ETL;
	using Retlang;
	using Rhino.ETL.Engine;

	internal class Program
	{
		private static void Main(string[] args)
		{
			try
			{
				EtlConfigurationContext ecc = new FakeEtlConfigurationContext();
				using (ecc.EnterContext())
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
					dd.Start(consumer, "Output");

					DataSource ds = new DataSource("test");
					ds.Execute(new IntGenerator(ds));
					ds.Start(producer, "Output");

					producer.Stop();
					producer.Join();
					factory.Stop();
					factory.Join();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}