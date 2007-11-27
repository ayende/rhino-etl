using System;
using System.Collections.Generic;

namespace Rhino.ETL
{
	using Retlang;

	public interface ICommandContainer
	{
		void Add(ICommand command);
		void WaitForCompletion(TimeSpan timeOut);

		IList<ICommand> Commands { get; }
		void ForceEndOfCompletionWithoutFurtherWait();
		void Execute(IProcessContextFactory context);
	}
}