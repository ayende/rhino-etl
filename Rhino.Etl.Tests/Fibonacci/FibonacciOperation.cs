using System.Collections.Generic;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;

namespace Rhino.Etl.Tests.Fibonacci
{
    public class FibonacciOperation : AbstractOperation
    {
        private readonly int max;

        public FibonacciOperation(int max)
        {
            this.max = max;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            int a = 0;
            int b = 1;
            var row = new Row();
            row["id"] = 1;
            yield return row;

            for (int i = 0; i < max - 1; i++)
            {
                int c = a + b;
                row = new Row();
                row["id"] = c;
                yield return row;

                a = b;
                b = c;
            }
        }
    }
}