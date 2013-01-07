using System.Data.SqlClient;
using Rhino.Etl.Core;

namespace Rhino.Etl.Tests.Fibonacci.Batch
{
    public class SlowBatchFibonacciToDatabase : BatchFibonacciToDatabaseBase
    {
        public SlowBatchFibonacciToDatabase()
            : base("test")
        {
            Timeout = 60;
        }

        protected override void PrepareCommand(Row row, SqlCommand command)
        {
            command.CommandText = "WAITFOR DELAY '00:00:02'; INSERT INTO Fibonacci (id) VALUES(@id)";
            command.Parameters.AddWithValue("@id", row["id"]);
        }

    }
}