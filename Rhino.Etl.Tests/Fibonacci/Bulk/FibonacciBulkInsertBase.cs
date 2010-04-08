using System.Configuration;
using Rhino.Etl.Core.Operations;

namespace Rhino.Etl.Tests.Fibonacci.Bulk
{
    public abstract class FibonacciBulkInsertBase : SqlBulkInsertOperation
    {
        protected FibonacciBulkInsertBase(string connectionString)
            : base(connectionString, "Fibonacci")
        {
        }

        protected FibonacciBulkInsertBase(ConnectionStringSettings connectionStringSettings)
            : base(connectionStringSettings, "Fibonacci")
        {
        }

        /// <summary>
        /// Prepares the schema of the target table
        /// </summary>
        protected override void PrepareSchema()
        {
            Schema["id"] = typeof (int);
        }
    }
}