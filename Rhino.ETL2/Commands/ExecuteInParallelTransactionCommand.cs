using System;
using System.Transactions;
using Rhino.ETL.Engine;

namespace Rhino.ETL.Commands
{
	using Retlang;

	public class ExecuteInParallelTransactionCommand : ExecuteInParallelCommand
	{
		private TransactionScope scope;

		public ExecuteInParallelTransactionCommand(Target target) : base(target)
		{
			scope = new TransactionScope();
		}

		public ExecuteInParallelTransactionCommand(Target target, IsolationLevel level) : base(target)
		{
			TransactionOptions transactionOptions = new TransactionOptions();
			transactionOptions.IsolationLevel = level;
			scope = new TransactionScope(TransactionScopeOption.Required,
				transactionOptions);
		}

		protected override void RegisterForExecution(ICommand command, IProcessContext context)
		{
			DependentTransaction dependentTransaction = Transaction.Current.DependentClone(DependentCloneOption.BlockCommitUntilComplete);
			context.Enqueue(delegate
			{
				using(TransactionScope tx = new TransactionScope(dependentTransaction))
				{
					command.Execute(context);
					tx.Complete();
				}
			});	
		}

		public override void WaitForCompletion(TimeSpan timeOut)
		{
			base.WaitForCompletion(timeOut);
			scope.Complete();
			scope.Dispose();
		}

		public override void ForceEndOfCompletionWithoutFurtherWait()
		{
			scope.Dispose();
			base.ForceEndOfCompletionWithoutFurtherWait();
		}
	}
}