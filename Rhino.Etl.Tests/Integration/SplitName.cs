namespace Rhino.Etl.Tests.Integration
{
    using System.Collections.Generic;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class SplitName : AbstractOperation
    {
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (Row row in rows)
            {
                string name = (string)row["name"];
                row["FirstName"] = name.Split()[0];
                row["LastName"] = name.Split()[1];
                yield return row;
            }
        }
    }
}