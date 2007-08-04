using System;

namespace Rhino.ETL.Commands
{
	public class ExecuteScalar : AbstractCommand, ICommandWithResult
	{
		private readonly string connectionName;
		private string command;
		private object result;

		public ExecuteScalar(string connectionName, string command)
		{
			this.connectionName = connectionName;
			this.command = command;
		}

		public string ConnectionName
		{
			get { return connectionName; }
		}

		public string Command
		{
			get { return command; }
			set { command = value; }
		}

		public object Result
		{
			get { return result; }
		}


		protected override void DoExecute()
		{
			result = 1;
			RaiseCompleted();
		}
	}
}
