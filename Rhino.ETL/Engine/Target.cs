using System;
using System.Collections.Generic;
using System.Threading;
using Boo.Lang;
using Boo.Lang.Compiler.MetaProgramming;
using Rhino.Commons;
using Rhino.ETL.Commands;
using Rhino.ETL.Exceptions;

namespace Rhino.ETL.Engine
{
	public abstract class Target : ContextfulObjectBase<Target>
	{
		private string name;
		private TimeSpan timeOut = TimeSpan.FromSeconds(5000);
		private ICommandContainer container;
		private List<Exception> exceptions = new List<Exception>();

		public Target(string name)
		{
			this.name = name;
			EtlConfigurationContext.Current.AddTarget(name, this);
			container = new ExecuteInParallelCommand(this);
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

		public bool IsFaulted
		{
			get { return exceptions.Count != 0; }
		}

		public ICommand Execute(string pipelineName)
		{
			Pipeline pipeline;
			if (EtlConfigurationContext.Current.Pipelines.TryGetValue(pipelineName, out pipeline) == false)
				throw new InvalidPipelineException("Could not find pipeline '" + pipelineName + "'");
			ExecutePipeline ep = new ExecutePipeline(this, pipeline);
			AddCommand(ep);
			return ep;
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
			ExecuteInParallelCommand command = new ExecuteInParallelCommand(this);
			RunUnderDifferentCommandContainerContext(command, command, callable);
		}

		[Meta]
		public void sequence(ICallable callable)
		{
			Sequence(callable);
		}

		[Meta]
		public void Sequence(ICallable callable)
		{
			ExecuteInSequenceCommand command = new ExecuteInSequenceCommand(this);
			RunUnderDifferentCommandContainerContext(command, command, callable);
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

		public void AddFault(Exception e)
		{
			exceptions.Add(e);
			container.ForceEndOfCompletionWithoutFurtherWait();
		}

		public ExecutionResult GetExecutionResult()
		{
			return new ExecutionResult(exceptions, 
				IsFaulted ? ExecutionStatus.Failure : ExecutionStatus.Success);
		}
	}
}
