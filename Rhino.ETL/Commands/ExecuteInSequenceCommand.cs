using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.ETL.Commands
{
	public class ExecuteInSequenceCommand : AbstractCommand, ICommandContainer
	{
		private bool started = false;
		List<ICommand> commands = new List<ICommand>();

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
			if (!started)
				throw new InvalidOperationException("Called WaitForCompletion before calling Execute");
		}

		protected override void DoExecute()
		{
			started = true;
			foreach (ICommand command in commands)
			{
				command.Execute();
			}
			RaiseCompleted();
		}
	}
}
