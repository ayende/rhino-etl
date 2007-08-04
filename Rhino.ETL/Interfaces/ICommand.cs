using System;
using System.Threading;

namespace Rhino.ETL
{
	public interface ICommand
	{
		event Action<ICommand> Completed;
		void Execute();
		WaitHandle GetWaitHandle();
		void After(ICommand command);
	}
}
