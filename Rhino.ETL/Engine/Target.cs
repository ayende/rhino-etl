using System;
using System.Collections.Generic;
using Boo.Lang;
using Boo.Lang.Compiler.MetaProgramming;
using Rhino.ETL.Commands;
using Rhino.ETL.Exceptions;

namespace Rhino.ETL.Engine
{
	public abstract class Target : ContextfulObjectBase<Target>
	{
		private string name;
		private TimeSpan timeOut = TimeSpan.FromSeconds(5000);
		private ICommandContainer container;

		public Target(string name)
		{
			this.name = name;
			EtlConfigurationContext.Current.AddTarget(name, this);
			container = new ExecuteInParallelCommand();
		}

		public override string Name
		{
			get { return name; }
		}

		public TimeSpan TimeOut
		{
			get { return timeOut; }
			set { timeOut = value; }
		}

		public IList<ICommand> Commands
		{
			get { return container.Commands; }
		}

		public void Execute(string pipelineName)
		{
			Pipeline pipeline;
			if (EtlConfigurationContext.Current.Pipelines.TryGetValue(pipelineName, out pipeline) == false)
				throw new InvalidPipelineException("Could not find pipeline '" + pipelineName + "'");
			ExecutePipeline ep = new ExecutePipeline(pipeline);
			AddCommand(ep);
		}

		private void AddCommand(ICommand command)
		{
			container.Add(command);
		}

		[Meta]
		public void parallel(ICallable callable)
		{
			Parallel(callable);
		}

		[Meta]
		public void Parallel(ICallable callable)
		{
			ExecuteInParallelCommand command = new ExecuteInParallelCommand();
			RunUnderDifferentCommandContainerContext(command,command, callable);
		}

		[Meta]
		public void sequence(ICallable callable)
		{
			Sequence(callable);
		}

		[Meta]
		public void Sequence(ICallable callable)
		{
			ExecuteInSequenceCommand command = new ExecuteInSequenceCommand();
			RunUnderDifferentCommandContainerContext(command,command, callable);
		}

		private void RunUnderDifferentCommandContainerContext(
			ICommand command,
			ICommandContainer newContainer, 
			ICallable callable)
		{
			ICommandContainer old = container;
			container = newContainer;
			callable.Call(new object[0]);
			container = old;
			container.Add(command);
		}

		public abstract void Prepare();

		public void Run()
		{
			container.Execute();
		}

		public void WaitForCompletion()
		{
			container.WaitForCompletion(TimeOut);
		}
	}
}
