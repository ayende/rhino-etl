using System;
using System.Collections.Generic;

namespace Rhino.ETL
{
	using Retlang;

	public interface ICommandContainer
	{
		void Add(ICommand command);
		bool WaitForCompletion(TimeSpan timeOut);
		void ForceEndOfCompletionWithoutFurtherWait();
		IList<ICommand> Commands { get; }
		void Execute(IProcessContextFactory context);
	}
}