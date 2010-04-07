using System.Collections.Generic;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace Rhino.Etl.Tests.Joins
{
    public class GenericEnumerableOperation : AbstractOperation
    {
        private readonly IEnumerable<Row> rowsToReturn;

        public GenericEnumerableOperation(IEnumerable<Row> rows)
        {
            rowsToReturn = rows;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            return rowsToReturn;
        }
    }
}