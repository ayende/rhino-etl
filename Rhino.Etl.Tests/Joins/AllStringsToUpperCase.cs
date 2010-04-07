namespace Rhino.Etl.Tests.Joins
{
    using System.Collections.Generic;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class AllStringsToUpperCase : AbstractOperation
    {
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (Row row in rows)
            {
                foreach (string column in row.Columns)
                {
                    string item = row[column] as string;
                    if(item!=null)
                        row[column] = item.ToUpper();
                }
                yield return row;
            }
        }
    }
}