using System.Configuration;

namespace Rhino.Etl.Tests.Fibonacci.Batch
{
    using System.Data.SqlClient;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class BatchFibonacciToDatabase : BatchFibonacciToDatabaseBase
    {
        public BatchFibonacciToDatabase() : base("test")
        {
        }
    }
}