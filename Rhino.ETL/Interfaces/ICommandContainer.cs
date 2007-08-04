using System;
using System.Collections.Generic;

namespace Rhino.ETL
{
	public interface ICommandContainer
	{
		void Add(ICommand command);
		void WaitForCompletion(TimeSpan timeOut);
		void Execute();

		IList<ICommand> Commands { get; }
		void ForceEndOfCompletionWithoutFurtherWait();
	}
}