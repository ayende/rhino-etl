namespace Rhino.Etl.Tests.Fibonacci.Bulk
{
    using Rhino.Etl.Core.Operations;

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