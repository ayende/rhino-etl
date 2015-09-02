using System.Configuration;
using Rhino.Etl.Core.Infrastructure;

namespace Rhino.Etl.Core.Operations
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    /// <summary>
    /// Perform a batch command against SQL server
    /// </summary>
    public abstract class SqlBatchOperation : AbstractDatabaseOperation
    {
        private int batchSize = 50;
        private int timeout = 30;

        /// <summary>
        /// Gets or sets the size of the batch.
        /// </summary>
        /// <value>The size of the batch.</value>
        public int BatchSize
        {
            get { return batchSize; }
            set { batchSize = value; }
        }

        /// <summary>
        /// The timeout of the command set
        /// </summary>
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBatchOperation"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        protected SqlBatchOperation(string connectionStringName)
            : this(ConfigurationManager.ConnectionStrings[connectionStringName])
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBatchOperation"/> class.
        /// </summary>
        /// <param name="connectionStringSettings">The connection string settings to use.</param>
        protected SqlBatchOperation(ConnectionStringSettings connectionStringSettings)
            : base(connectionStringSettings)
        {
            base.paramPrefix = "@";
        }

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            Guard.Against<ArgumentException>(rows == null, "SqlBatchOperation cannot accept a null enumerator");
            using (SqlConnection connection = (SqlConnection)Use.Connection(ConnectionStringSettings))
            using (SqlTransaction transaction = (SqlTransaction) BeginTransaction(connection))
            {
                SqlCommandSet commandSet = null;
                CreateCommandSet(connection, transaction, ref commandSet, timeout);

                foreach (Row row in rows)
                {
                    SqlCommand command = new SqlCommand();
                    PrepareCommand(row, command);
                    if (command.Parameters.Count == 0 && (RuntimeInfo.Version.Contains("2.0") || RuntimeInfo.Version.Contains("1.1"))) //workaround around a framework bug
                    {
                        Guid guid = Guid.NewGuid();
                        command.Parameters.AddWithValue(guid.ToString(), guid);
                    }
                    commandSet.Append(command);
                    if (commandSet.CountOfCommands >= batchSize)
                    {
                        Debug("Executing batch of {0} commands", commandSet.CountOfCommands);
                        commandSet.ExecuteNonQuery();
                        CreateCommandSet(connection, transaction, ref commandSet, timeout);
                    }
                }
                Debug("Executing final batch of {0} commands", commandSet.CountOfCommands);
                commandSet.ExecuteNonQuery();

                if (PipelineExecuter.HasErrors)
                {
                    Warn(null, "Rolling back transaction in {0}", Name);
                    if (transaction != null) transaction.Rollback();
                    Warn(null, "Rolled back transaction in {0}", Name);
                }
                else
                {
                    Debug("Committing {0}", Name);
                    if (transaction != null) transaction.Commit();
                    Debug("Committed {0}", Name);
                }                    

            }
            yield break;
        }

        /// <summary>
        /// Prepares the command from the given row
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="command">The command.</param>
        protected abstract void PrepareCommand(Row row, SqlCommand command);

        private static void CreateCommandSet(SqlConnection connection, SqlTransaction transaction, ref SqlCommandSet commandSet, int timeout)
        {
            if (commandSet != null)
                commandSet.Dispose();
            commandSet = new SqlCommandSet
            {
                Connection = connection, 
                Transaction = transaction,
                CommandTimeout = timeout
            };
        }
    }
}