using Rhino.Etl.Core.Enumerables;

namespace Rhino.Etl.Core.Operations
{
    using System.Collections.Generic;
    using System.Data;
    using Commons;

    /// <summary>
    /// Generic output command operation
    /// </summary>
    public abstract class OutputCommandOperation : AbstractCommandOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutputCommandOperation"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        public OutputCommandOperation(string connectionStringName) : base(connectionStringName)
        {
        }

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            using (IDbConnection connection = Use.Connection(ConnectionStringName))
            using (IDbTransaction transaction = connection.BeginTransaction())
            {
                foreach (Row row in new SingleRowEventRaisingEnumerator(this, rows))
                {
                    using (IDbCommand cmd = connection.CreateCommand())
                    {
                        currentCommand = cmd;
                        currentCommand.Transaction = transaction;
                        PrepareCommand(currentCommand, row);
                        currentCommand.ExecuteNonQuery();
                    }
                }
                if (PipelineExecuter.HasErrors)
                {
                    Warn("Rolling back transaction in {0}", Name);
                    transaction.Rollback();
                    Warn("Rolled back transaction in {0}", Name);
                }
                else
                {
                    Debug("Committing {0}", Name);
                    transaction.Commit();
                    Debug("Committed {0}", Name);
                }
            }
            yield break;
        }

        /// <summary>
        /// Prepares the command for execution, set command text, parameters, etc
        /// </summary>
        /// <param name="cmd">The command.</param>
        /// <param name="row">The row.</param>
        protected abstract void PrepareCommand(IDbCommand cmd, Row row);
    }
}
