using System;
using System.Collections.Generic;
using Rhino.Commons;
using Rhino.ETL.Engine;

namespace Rhino.ETL.Commands
{
	public class ExecuteInParallelCommand : AbstractCommand, ICommandContainer
	{
		protected List<ICommand> commands = new List<ICommand>();
		protected CountdownLatch latch;

		public ExecuteInParallelCommand(Target target)
			: base(target)
		{
		}

		public IList<ICommand> Commands
		{
			get { return commands; }
		}

		public virtual void ForceEndOfCompletionWithoutFurtherWait()
		{
			int remaining;
			do
			{
				remaining = latch.Set();
			} while (remaining > 0);
		}

		public void Add(ICommand command)
		{
			commands.Add(command);
		}

		public virtual void WaitForCompletion(TimeSpan timeOut)
		{
			if (latch == null)
				throw new InvalidOperationException("Called WaitForCompletion before calling Execute");
			latch.WaitOne(timeOut);
		}

		protected override void DoExecute()
		{
			latch = new CountdownLatch(commands.Count);
			foreach (ICommand command in commands)
			{
				command.Completed += delegate
				{
					int remaining = latch.Set();
					if (remaining == 0)
						RaiseCompleted();
				};
				RegisterForExecution(command);
			}
		}

		protected virtual void RegisterForExecution(ICommand command)
		{
			ExecutionPackage.Current.RegisterForExecution(target, command.Execute);
		}
	}
}