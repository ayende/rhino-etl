namespace Rhino.ETL.Commands
{
	using System;
	using System.Transactions;
	using Engine;
	using log4net.Repository.Hierarchy;
	using Retlang;

	public class ExecuteInParallelTransactionCommand : ExecuteInParallelCommand
	{
		private readonly IsolationLevel level;
		private TransactionScope scope;

		public ExecuteInParallelTransactionCommand(Target target)
			: this(target, IsolationLevel.ReadCommitted)
		{
		}

		public ExecuteInParallelTransactionCommand(Target target, IsolationLevel level)
			: base(target)
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

		protected override void ExecuteCommand(IProcessContextFactory contextFactory, ICommandQueue context, ICommand command)
		{
			EtlConfigurationContext configurationContext = EtlConfigurationContext.Current;
			if (Transaction.Current == null)
				throw new InvalidOperationException("You are not running in a transaction!");
			DependentTransaction dependentTransaction = Transaction.Current.DependentClone(DependentCloneOption.BlockCommitUntilComplete);
			context.Enqueue(delegate
			{
				using (TransactionScope tx = new TransactionScope(dependentTransaction))
				{
					IProcessContext reportStatusContext = contextFactory.CreateAndStart();
					command.Completed += OnCommandCompleted(reportStatusContext);
					try
					{
						using (configurationContext.EnterContext())
						{
							command.Execute(contextFactory);
							tx.Complete();
						}
					}
					catch (Exception ex)
					{
						dependentTransaction.Rollback(ex);
						reportStatusContext.Publish(Messages.Exception, ex);
						logger.Error("Failed to execute command " + command, ex);
					}
					finally
					{
						reportStatusContext.Stop();
					}
				}
			});
		}

		public override bool WaitForCompletion(TimeSpan timeOut)
		{
			if (base.WaitForCompletion(timeOut) == false)
				return false;
			scope.Complete();
			scope.Dispose();
			return true;
		}

		public override void ForceEndOfCompletionWithoutFurtherWait()
		{
			scope.Dispose();
			base.ForceEndOfCompletionWithoutFurtherWait();
		}
	}
}