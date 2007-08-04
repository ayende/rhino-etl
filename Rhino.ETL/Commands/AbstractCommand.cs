using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rhino.ETL.Commands
{
	public abstract class AbstractCommand : ICommand
	{
		public bool HasCompleted = false;
		private List<ICommand> commandsThatMustBeCompletedBeforeThisCommandCanRun = new List<ICommand>();
		public event Action<ICommand> Completed;
		public AbstractCommand()
		{
			Completed += delegate { HasCompleted = true; };
		}

		public IList<ICommand> CommandsThatMustBeCompletedBeforeThisCommandCanRun
		{
			get { return commandsThatMustBeCompletedBeforeThisCommandCanRun; }
		}

		public void Execute()
		{
			foreach (ICommand command in commandsThatMustBeCompletedBeforeThisCommandCanRun)
			{
				using (WaitHandle handle = command.GetWaitHandle())
					handle.WaitOne();
			}
			DoExecute();
		}

		protected abstract void DoExecute();

		public WaitHandle GetWaitHandle()
		{
			ManualResetEvent resetEvent = new ManualResetEvent(false);
			Completed += delegate
			{
				resetEvent.Set();
			};
			if (HasCompleted)
				resetEvent.Set();
			return resetEvent;
		}

		public void After(ICommand command)
		{
			commandsThatMustBeCompletedBeforeThisCommandCanRun.Add(command);
		}

		protected void RaiseCompleted()
		{
			Completed(this);
		}
	}
}
