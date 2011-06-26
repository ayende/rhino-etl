using System.Configuration;
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
            : this(ConfigurationManager.ConnectionStrings[connectionStringName])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputCommandOperation"/> class.
        /// </summary>
        /// <param name="connectionStringSettings">Connection string settings to use.</param>
        public InputCommandOperation(ConnectionStringSettings connectionStringSettings)
            : base(connectionStringSettings)
        {
            UseTransaction = true;
        }
       
        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            using (IDbConnection connection = Use.Connection(ConnectionStringSettings))
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

                if (transaction != null) transaction.Commit();
            }
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
