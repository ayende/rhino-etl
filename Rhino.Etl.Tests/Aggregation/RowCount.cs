namespace Rhino.Etl.Tests.Aggregation
{
    using Core;
    using Rhino.Etl.Core.Operations;

    public class RowCount : AbstractAggregationOperation
    {
        /// <summary>
        /// Accumulate the current row to the current aggregation
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="aggregate">The aggregate.</param>
        protected override void Accumulate(Row row, Row aggregate)
        {
            if (aggregate["count"] == null)
                aggregate["count"] = 0;

            int count = (int)aggregate["count"];
            aggregate["count"] = count + 1;
        }
    }
}