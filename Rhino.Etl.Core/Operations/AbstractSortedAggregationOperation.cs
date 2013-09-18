namespace Rhino.Etl.Core.Operations
{
    using System.Collections.Generic;

    /// <summary>
    /// An aggregation operation, handles all the backend stuff of the aggregation,
    /// leaving client code just the accumulation process. Assumes a sorted rowset
    /// so that we can return early instead of having to accumulate all rows.
    /// </summary>
    public abstract class AbstractSortedAggregationOperation : AbstractAggregationOperation
    {
        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows">The pre-sorted rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            ObjectArrayKeys previousKey = null;
            var aggregate = new Row();
            var groupBy = GetColumnsToGroupBy();

            foreach (var row in rows)
            {
                var key = row.CreateKey(groupBy);

                if (previousKey != null && !previousKey.Equals(key))
                {
                    FinishAggregation(aggregate);
                    yield return aggregate;
                    aggregate = new Row();
                }

                Accumulate(row, aggregate);
                previousKey = key;
            }

            FinishAggregation(aggregate);
            yield return aggregate;
        }
    }
}