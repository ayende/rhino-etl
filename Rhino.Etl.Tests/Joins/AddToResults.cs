namespace Rhino.Etl.Tests.Joins
{
    using System.Collections.Generic;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class AddToResults : AbstractOperation
    {
        private readonly ICollection<Row> results;

        public AddToResults(ICollection<Row> results)
        {
            this.results = results;
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (Row row in rows)
            {
                results.Add(row);
            }
            return results;
        }
    }
}