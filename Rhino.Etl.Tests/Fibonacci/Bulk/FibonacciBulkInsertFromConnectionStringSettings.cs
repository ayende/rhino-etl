using System.Configuration;
using Rhino.Etl.Core.Operations;

namespace Rhino.Etl.Tests.Fibonacci.Bulk
{
    public class FibonacciBulkInsertFromConnectionStringSettings : FibonacciBulkInsertBase
    {
        public FibonacciBulkInsertFromConnectionStringSettings()
            : base(ConfigurationManager.ConnectionStrings["test"])
        {
        }
    }
}