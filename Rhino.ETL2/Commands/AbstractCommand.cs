using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Rhino.ETL.Engine;

namespace Rhino.ETL.Commands
{
	using Retlang;

	public abstract class AbstractCommand : ICommand
	{
		protected readonly Target target;
		public bool HasCompleted = false;
		private readonly List<ICommand> commandsThatMustBeCompletedBeforeThisCommandCanRun = new List<ICommand>();
		public event Action<ICommand> Completed;
		public AbstractCommand(Target target)
		{
			this.target = target;
			Completed += delegate { HasCompleted = true; };
		}

		public IList<ICommand> CommandsThatMustBeCompletedBeforeThisCommandCanRun
		{
			get { return commandsThatMustBeCompletedBeforeThisCommandCanRun; }
		}

		public void Execute(IProcessContext context)
		{
			foreach (ICommand command in commandsThatMustBeCompletedBeforeThisCommandCanRun)
			{
				using (WaitHandle handle = command.GetWaitHandle())
					handle.WaitOne();
			}
			DoExecute(context);
		}

		protected abstract void DoExecute(IProcessContext context);

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
