namespace Rhino.Etl.Tests.Errors
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Core;
    using Core.Operations;

    public class ThrowingOperation : AbstractOperation
    {
        private readonly int rowsAfterWhichToThrow = new Random().Next(1, 6);

        public int RowsAfterWhichToThrow
        {
            get { return rowsAfterWhichToThrow; }
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            for (int i = 0; i < RowsAfterWhichToThrow; i++)
            {
                Row row = new Row();
                row["id"] = i;
                yield return row;
            }
            throw new InvalidDataException("problem");
        }
    }
}
