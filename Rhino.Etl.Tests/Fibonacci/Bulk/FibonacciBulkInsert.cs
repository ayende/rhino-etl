using Rhino.Etl.Core.Operations;

namespace Rhino.Etl.Tests.Fibonacci.Bulk
{
    public class FibonacciBulkInsert : SqlBulkInsertOperation
    {
        public FibonacciBulkInsert() : base("test", "Fibonacci")
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