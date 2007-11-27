using System;
using System.Transactions;
using Rhino.ETL.Engine;

namespace Rhino.ETL.Commands
{
	using Retlang;

	public class ExecuteInParallelTransactionCommand : ExecuteInParallelCommand
	{
		private readonly IsolationLevel level;
		private TransactionScope scope;

		public ExecuteInParallelTransactionCommand(Target target) : this(target, IsolationLevel.ReadCommitted)
		{
		}

		public ExecuteInParallelTransactionCommand(Target target, IsolationLevel level) : base(target)
		{
			this.level = level;
		}


		protected override void BeforeExecutingCommands(IProcessContext context)
		{
			TransactionOptions transactionOptions = new TransactionOptions();
			transactionOptions.IsolationLevel = level;
			scope = new TransactionScope(TransactionScopeOption.Required,
				transactionOptions);

			context.Subscribe<Exception>(new TopicEquals(Messages.Exception),
			                             delegate(IMessageHeader header, Exception msg)
			                             {
											 Console.WriteLine(msg);
			                             });
		}

		protected override void RegisterForExecution(ICommand command,IProcessContextFactory contextFactory, IProcessContext context)
		{
			EtlConfigurationContext current = EtlConfigurationContext.Current;
			if (Transaction.Current == null)
				throw new InvalidOperationException("You are not running in a transaction!");
			DependentTransaction dependentTransaction = Transaction.Current.DependentClone(DependentCloneOption.BlockCommitUntilComplete);
			context.Enqueue(delegate
			{
				using(current.EnterContext())
				using(TransactionScope tx = new TransactionScope(dependentTransaction))
				{
					try
					{
						command.Execute(contextFactory);
						tx.Complete();
					}
					catch (Exception ex)
					{
						dependentTransaction.Rollback(ex);
						throw;
					}
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