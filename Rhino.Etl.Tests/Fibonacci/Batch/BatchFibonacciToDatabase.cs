namespace Rhino.Etl.Tests.Fibonacci.Batch
{
    using System.Data.SqlClient;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class BatchFibonacciToDatabase : SqlBatchOperation
    {
        public BatchFibonacciToDatabase() : base("test")
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