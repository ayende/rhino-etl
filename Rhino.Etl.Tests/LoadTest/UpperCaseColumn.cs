namespace Rhino.Etl.Tests.LoadTest
{
    using System.Collections.Generic;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class UpperCaseColumn : AbstractOperation
    {
        private readonly string column;

        public UpperCaseColumn(string column)
        {
            this.column = column;
        }

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (Row row in rows)
            {
                row[column] = ((string) row[column] ?? "").ToUpper();
                row["testMsg"] = "UpperCased";
                yield return row;
            }
        }
    }
}