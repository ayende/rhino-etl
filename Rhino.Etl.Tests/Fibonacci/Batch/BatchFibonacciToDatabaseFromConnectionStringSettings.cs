using System.Configuration;

namespace Rhino.Etl.Tests.Fibonacci.Batch
{
    using System.Data.SqlClient;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class BatchFibonacciToDatabaseFromConnectionStringSettings : BatchFibonacciToDatabaseBase
    {
        public BatchFibonacciToDatabaseFromConnectionStringSettings()
            : base(ConfigurationManager.ConnectionStrings["test"])
        {
        }
    }
}