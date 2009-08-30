using System;
using System.Configuration;
using System.Data;

namespace Rhino.Etl.Core.Infrastructure
{
	/// <summary>
	/// Helper class to provide simple data access, when we want to access the ADO.Net
	/// library directly. 
	/// </summary>
	public static class Use
	{
		#region Delegates

		/// <summary>
		/// Delegate to execute an action with a command
		/// and return a result: <typeparam name="T"/>
		/// </summary>
		public delegate T Func<T>(IDbCommand command);

		/// <summary>
		/// Delegate to execute an action with a command
		/// </summary>
		public delegate void Proc(IDbCommand command);

		#endregion

		private static readonly object activeConnectionKey = new object();
		private static readonly object activeTransactionKey = new object();
		private static readonly object transactionCounterKey = new object();

		/// <summary>
		/// Gets or sets the active connection.
		/// </summary>
		/// <value>The active connection.</value>
		[ThreadStatic]
		private static IDbConnection ActiveConnection;

		/// <summary>
		/// Gets or sets the active transaction.
		/// </summary>
		/// <value>The active transaction.</value>
		[ThreadStatic] 
		private static IDbTransaction ActiveTransaction;

		/// <summary>
		/// Gets or sets the transaction counter.
		/// </summary>
		/// <value>The transaction counter.</value>
		[ThreadStatic]
		private static int TransactionCounter;

		/// <summary>
		/// Execute the specified delegate inside a transaction and return 
		/// the result of the delegate.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="connectionStringName">The name of the named connection string in the configuration file</param>
		/// <param name="actionToExecute">The action to execute</param>
		/// <returns></returns>
		public static T Transaction<T>(string connectionStringName, Func<T> actionToExecute)
		{
			T result = default(T);
			Transaction(connectionStringName, delegate(IDbCommand command) { result = actionToExecute(command); });
			return result;
		}

		/// <summary>
		/// Execute the specified delegate inside a transaction
		/// </summary>
		/// <param name="connectionStringName">Name of the connection string.</param>
		/// <param name="actionToExecute">The action to execute.</param>
		public static void Transaction(string connectionStringName, Proc actionToExecute)
		{
			Transaction(connectionStringName, IsolationLevel.Unspecified, actionToExecute);
		}

		/// <summary>
		/// Execute the specified delegate inside a transaction with the specific
		/// isolation level 
		/// </summary>
		/// <param name="connectionStringName">Name of the connection string.</param>
		/// <param name="isolationLevel">The isolation level.</param>
		/// <param name="actionToExecute">The action to execute.</param>
		public static void Transaction(string connectionStringName, IsolationLevel isolationLevel, Proc actionToExecute)
		{
			StartTransaction(connectionStringName, isolationLevel);
			try
			{
				using (IDbCommand command = ActiveConnection.CreateCommand())
				{
					command.Transaction = ActiveTransaction;
					actionToExecute(command);
				}
				CommitTransaction();
			}
			catch
			{
				RollbackTransaction();
				throw;
			}
			finally
			{
				DisposeTransaction();
			}
		}

		/// <summary>
		/// Disposes the transaction.
		/// </summary>
		private static void DisposeTransaction()
		{
			if (TransactionCounter <= 0)
			{
				ActiveConnection.Dispose();
				ActiveConnection = null;
			}
		}

		/// <summary>
		/// Rollbacks the transaction.
		/// </summary>
		private static void RollbackTransaction()
		{
			ActiveTransaction.Rollback();
			ActiveTransaction.Dispose();
			ActiveTransaction = null;
			TransactionCounter = 0;
		}

		/// <summary>
		/// Commits the transaction.
		/// </summary>
		private static void CommitTransaction()
		{
			TransactionCounter--;
			if (TransactionCounter == 0 && ActiveTransaction != null)
			{
				ActiveTransaction.Commit();
				ActiveTransaction.Dispose();
				ActiveTransaction = null;
			}
		}

		/// <summary>
		/// Starts the transaction.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="isolation">The isolation.</param>
		private static void StartTransaction(string name, IsolationLevel isolation)
		{
			if (TransactionCounter <= 0)
			{
				TransactionCounter = 0;
				ActiveConnection = Connection(name);
				ActiveTransaction = ActiveConnection.BeginTransaction(isolation);
			}
			TransactionCounter++;
		}

		/// <summary>
		/// Creates an open connection for a given named connection string, using the provider name
		/// to select the proper implementation
		/// </summary>
		/// <param name="name">The name.</param>
		/// <returns>The open connection</returns>
		public static IDbConnection Connection(string name)
		{
			ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings[name];
			if (connectionString == null)
				throw new InvalidOperationException("Could not find connnection string: " + name);
			Type type = Type.GetType(connectionString.ProviderName);
			if (type == null)
				throw new InvalidOperationException("The type name '" + connectionString.ProviderName +
				                                    "' could not be found for connection string: " + name);
			IDbConnection connection = (IDbConnection) Activator.CreateInstance(type);
			connection.ConnectionString = connectionString.ConnectionString;
			connection.Open();
			return connection;
		}
	}
}