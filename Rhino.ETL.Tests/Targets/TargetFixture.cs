using System;
using System.Threading;
using MbUnit.Framework;
using Rhino.ETL.Commands;
using Rhino.ETL.Engine;
using Rhino.ETL.Tests.Joins;

namespace Rhino.ETL.Tests.Targets
{
	[TestFixture]
	public class TargetFixture : BaseTest
	{
		private EtlConfigurationContext configurationContext;

		[SetUp]
		public void TestInitialize()
		{
			configurationContext = BuildContext(@"Targets\targets.retl");
		}
		
		[Test]
		public void CanSpecifyParallelExecution()
		{
			Target target = configurationContext.Targets["default"];
			using(configurationContext.EnterContext())
				target.Prepare();
			ExecuteInParallelCommand command = (ExecuteInParallelCommand)target.Commands[0];
			Assert.AreEqual(2, command.Commands.Count);
		}

		[Test]
		public void CanSpecifySequenceExecution()
		{
			Target target = configurationContext.Targets["default"];
			using (configurationContext.EnterContext())
				target.Prepare();
			ExecuteInSequenceCommand command = (ExecuteInSequenceCommand)target.Commands[1];
			Assert.AreEqual(2, command.Commands.Count);
		}

		[Test]
		public void CanSpecifyAfterDependecy()
		{
			Target target = configurationContext.Targets["withDependencies"];
			using (configurationContext.EnterContext())
				target.Prepare();
			AbstractCommand command = (AbstractCommand)target.Commands[2];
			Assert.AreEqual(
				target.Commands[0], 
				command.CommandsThatMustBeCompletedBeforeThisCommandCanRun[0]);
		}

		[Test]
		public void CommandWillWaitForDependentCommandBeforeRunning()
		{
			Target target = configurationContext.Targets["withDependencies"];
			using (configurationContext.EnterContext())
				target.Prepare();
			AbstractCommand mustRunFirst = (AbstractCommand)target.Commands[0];
			AbstractCommand dependant = (AbstractCommand)target.Commands[2];

			bool dependantCompleted = false;
			bool mustRunFirstCompleted = false;

			mustRunFirst.Completed += delegate
			{
				Assert.IsFalse(dependantCompleted);
				mustRunFirstCompleted = true;
			};

			dependant.Completed += delegate
			{
				Assert.IsTrue(mustRunFirstCompleted);
				dependantCompleted = true;
			};

			WaitHandle dependantWaitHandle = dependant.GetWaitHandle();
			WaitHandle mustrunfirstWaitHandle = mustRunFirst.GetWaitHandle();

			ThreadPool.QueueUserWorkItem(delegate
			{
				using (new TestExecutionPackage().EnterContext()) 
				using (configurationContext.EnterContext())
					dependant.Execute();
			});
			Thread.Sleep(250);
			ThreadPool.QueueUserWorkItem(delegate
			{
				using (new TestExecutionPackage().EnterContext())
				using (configurationContext.EnterContext())
					mustRunFirst.Execute();
			});

			WaitHandle.WaitAll(new WaitHandle[] {mustrunfirstWaitHandle, dependantWaitHandle});
			Assert.IsTrue(mustRunFirstCompleted);
			Assert.IsTrue(dependantCompleted);
		}
	}
}
