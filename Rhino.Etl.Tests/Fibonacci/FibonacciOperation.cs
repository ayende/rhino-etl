namespace Rhino.Etl.Tests.Fibonacci
{
    using System.Collections.Generic;
    using Core;
    using Rhino.Etl.Core.Operations;

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
            Row row = new Row();
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