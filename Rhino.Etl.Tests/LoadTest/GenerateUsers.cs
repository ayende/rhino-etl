namespace Rhino.Etl.Tests.LoadTest
{
    using System.Collections.Generic;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class GenerateUsers : AbstractOperation
    {
        public GenerateUsers(int expectedCount)
        {
            this.expectedCount = expectedCount;
        }

        private int expectedCount;

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            for (int i = 0; i < expectedCount; i++)
            {
                Row row = new Row();
            	row["id"] = i;
                row["name"] = "ayende #" + i;
                row["email"] = "ayende" + i + "@example.org";
                yield return row;
            }
        }
    }
}