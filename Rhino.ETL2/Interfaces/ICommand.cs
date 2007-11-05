namespace Rhino.ETL
{
	using System;
	using System.Threading;
	using Retlang;

	public interface ICommand
	{
		event Action<ICommand> Completed;
		void Execute(IProcessContextFactory context);
		WaitHandle GetWaitHandle();
		void After(ICommand command);
	}
}
