using System;
using System.Collections.Generic;

namespace Rhino.ETL.Engine
{
	public class ExecutionResult
	{
		private readonly List<string> errors;
		private readonly List<Exception> exceptions = new List<Exception>();
		private readonly ExecutionStatus status;


		public ExecutionResult(ExecutionStatus status, List<string> errors)
		{
			this.status = status;
			this.errors = errors;
		}

		public ExecutionResult(List<Exception> exceptions, ExecutionStatus status, List<string> errors)
		{
			this.exceptions = exceptions;
			this.errors = errors;
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