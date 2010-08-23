namespace Rhino.Etl.Tests.Dsl
{
    using System.Collections.Generic;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class ResultsToList : AbstractOperation
    {
        public List<Row> Results;

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            Results = new List<Row>(rows);
            yield break;
        }
    }
}