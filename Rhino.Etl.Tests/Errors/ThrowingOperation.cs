namespace Rhino.Etl.Tests.Errors
{
    using System.Collections.Generic;
    using System.IO;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class ThrowingOperation : AbstractOperation
    {
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            for (int i = 0; i < 2; i++)
            {
                Row row = new Row();
                row["id"] = i;
                yield return row;
            }
            throw new InvalidDataException("problem");
        }
    }
}