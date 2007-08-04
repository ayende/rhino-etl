using System;
using System.Collections.Generic;

namespace Rhino.ETL
{
	public class ExecutionResult
	{
		private List<Exception> exceptions = new List<Exception>();
		private ExecutionStatus status;


		public ExecutionResult(ExecutionStatus status)
		{
			this.status = status;
		}

		public ExecutionResult(List<Exception> exceptions, ExecutionStatus status)
		{
			this.exceptions = exceptions;
			this.status = status;
		}


		public List<Exception> Exceptions
		{
			get { return exceptions; }
		}

		public ExecutionStatus Status
		{
			get { return status; }
		}
	}
}