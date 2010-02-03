using Rhino.Etl.Core.Infrastructure;

namespace Rhino.Etl.Core.Operations
{
    using System.Collections.Generic;
    using System.Data;

	/// <summary>
    /// Generic input command operation
    /// </summary>
    public abstract class InputCommandOperation : AbstractCommandOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputCommandOperation"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        public InputCommandOperation(string connectionStringName)
            : base(connectionStringName)
        {
            UseTransaction = true;
        }

        ///<summary>
        /// True, if the input operation should be run in a transaction. Otherwise,false.
        ///</summary>
        public bool UseTransaction
        {
            get;
            set;
        }

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            using (IDbConnection connection = Use.Connection(ConnectionStringName))
            using (IDbTransaction transaction = BeginTransaction(connection))
            {
                using (currentCommand = connection.CreateCommand())
                {
                    currentCommand.Transaction = transaction;
                    PrepareCommand(currentCommand);
                    using (IDataReader reader = currentCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return CreateRowFromReader(reader);
                        }
                    }
                }
                if (transaction != null)
                {
                    transaction.Commit();
                }
            }
        }

        IDbTransaction BeginTransaction(IDbConnection connection)
        {
            if (UseTransaction)
            {
                return connection.BeginTransaction();
            }
            return null;
        }

        /// <summary>
        /// Creates a row from the reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        protected abstract Row CreateRowFromReader(IDataReader reader);

        /// <summary>
        /// Prepares the command for execution, set command text, parameters, etc
        /// </summary>
        /// <param name="cmd">The command.</param>
        protected abstract void PrepareCommand(IDbCommand cmd);
    }
}
