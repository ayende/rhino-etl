using System.Configuration;

namespace Rhino.Etl.Tests.Fibonacci.Batch
{
    using System.Data.SqlClient;
    using Core;
    using Rhino.Etl.Core.Operations;

    public abstract class BatchFibonacciToDatabaseBase : SqlBatchOperation
    {
        protected BatchFibonacciToDatabaseBase(string connectionString)
            : base(connectionString)
        {
        }

        protected BatchFibonacciToDatabaseBase(ConnectionStringSettings connectionStringSettings)
            : base(connectionStringSettings)
        {
        }

        /// <summary>
        /// Prepares the command from the given row
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="command">The command.</param>
        protected override void PrepareCommand(Row row, SqlCommand command)
        {
            command.CommandText = "INSERT INTO Fibonacci (id) VALUES(@id)";
            command.Parameters.AddWithValue("@id", row["id"]);
        }
    }
}