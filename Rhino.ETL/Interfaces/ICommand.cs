using System;
using System.Collections.Generic;
using System.Text;

namespace Rhino.ETL
{
	public interface ICommand
	{
		event Action<ICommand> Completed;
		void Execute();
	}
}
