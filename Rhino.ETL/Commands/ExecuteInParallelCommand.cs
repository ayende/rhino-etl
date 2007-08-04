using System;
using System.Collections.Generic;
using Rhino.Commons;

namespace Rhino.ETL.Commands
{
	public class ExecuteInParallelCommand :  ICommand, ICommandContainer
	{
		public event Action<ICommand> Completed = delegate { };
		protected List<ICommand> commands = new List<ICommand>();
		protected CountdownLatch latch;


		public IList<ICommand> Commands
		{
			get { return commands; }
		}

		public void Add(ICommand command)
		{
			commands.Add(command);
		}

		public void WaitForCompletion(TimeSpan timeOut)
		{
			if (latch==null)
				throw new InvalidOperationException("Called WaitForCompletion before calling Execute");
			latch.WaitOne(timeOut);
		}

		public void Execute()
		{
			latch = new CountdownLatch(commands.Count);
			foreach (ICommand command in commands)
			{
				command.Completed += delegate
				{
					int remaining = latch.Set();
					if (remaining == 0)
						Completed(this);
				};
				ExecutionPackage.Current.RegisterForExecution(command.Execute);
			}
		}
	}
}