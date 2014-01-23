using System;
using System.Configuration;
using System.Data;

namespace Rhino.Etl.Core.Infrastructure
{
    using System.Data.Common;

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

            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionStringSettings == null)
                throw new InvalidOperationException("Could not find connnection string: " + connectionStringName);

            Transaction(connectionStringSettings, delegate(IDbCommand command) { result = actionToExecute(command); });
            return result;
        }

        /// <summary>
        /// Execute the specified delegate inside a transaction and return 
        /// the result of the delegate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionStringSettings">The connection string settings to use for the connection</param>
        /// <param name="actionToExecute">The action to execute</param>
        /// <returns></returns>
        public static T Transaction<T>(ConnectionStringSettings connectionStringSettings, Func<T> actionToExecute)
        {
            T result = default(T);
            Transaction(connectionStringSettings, delegate(IDbCommand command) { result = actionToExecute(command); });
            return result;
        }

        /// <summary>
        /// Execute the specified delegate inside a transaction
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        /// <param name="actionToExecute">The action to execute.</param>
        public static void Transaction(string connectionStringName, Proc actionToExecute)
        {
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionStringSettings == null)
                throw new InvalidOperationException("Could not find connnection string: " + connectionStringName);

            Transaction(connectionStringSettings, IsolationLevel.Unspecified, actionToExecute);
        }

        /// <summary>
        /// Execute the specified delegate inside a transaction
        /// </summary>
        /// <param name="connectionStringSettings">The connection string settings to use for the connection</param>
        /// <param name="actionToExecute">The action to execute.</param>
        public static void Transaction(ConnectionStringSettings connectionStringSettings, Proc actionToExecute)
        {
            Transaction(connectionStringSettings, IsolationLevel.Unspecified, actionToExecute);
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
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionStringSettings == null)
                throw new InvalidOperationException("Could not find connnection string: " + connectionStringName);

            Transaction(connectionStringSettings, isolationLevel, actionToExecute);
        }

        /// <summary>
        /// Execute the specified delegate inside a transaction with the specific
        /// isolation level 
        /// </summary>
        /// <param name="connectionStringSettings">Connection string settings node to use for the connection</param>
        /// <param name="isolationLevel">The isolation level.</param>
        /// <param name="actionToExecute">The action to execute.</param>
        public static void Transaction(ConnectionStringSettings connectionStringSettings, IsolationLevel isolationLevel, Proc actionToExecute)
        {
            StartTransaction(connectionStringSettings, isolationLevel);
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
        /// <param name="connectionStringSettings">The connection string settings to use for the transaction</param>
        /// <param name="isolation">The isolation.</param>
        private static void StartTransaction(ConnectionStringSettings connectionStringSettings, IsolationLevel isolation)
        {
            if (TransactionCounter <= 0)
            {
                TransactionCounter = 0;
                ActiveConnection = Connection(connectionStringSettings);
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

            return Connection(connectionString);
        }

        /// <summary>
        /// Creates an open connection for a given connection string setting, using the provider
        /// name of select the proper implementation
        /// </summary>
        /// <param name="connectionString">ConnectionStringSetting node</param>
        /// <returns>The open connection</returns>
        public static IDbConnection Connection(ConnectionStringSettings connectionString)
        {
            if (connectionString == null)
                throw new InvalidOperationException("Null ConnectionStringSettings specified");
            if (connectionString.ProviderName == null)
                throw new InvalidOperationException("Null ProviderName specified");

            IDbConnection connection = null;

            string providerName = connectionString.ProviderName;
            if (providerName != null)
            {
                // Backwards compatibility: ProviderName could be an assembly qualified connection type name.
                Type connectionType = Type.GetType(providerName);
                if (connectionType != null)
                {
                    connection = Activator.CreateInstance(connectionType) as IDbConnection;
                }
            }
            if (connection == null)
            {
                // ADO.NET compatible usage of provider name.
                connection = DbProviderFactories.GetFactory(providerName).CreateConnection();
            }

            connection.ConnectionString = connectionString.ConnectionString;
            connection.Open();
            return connection;
        }
    }
}