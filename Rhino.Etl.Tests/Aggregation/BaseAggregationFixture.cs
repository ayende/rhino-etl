namespace Rhino.Etl.Tests.Aggregation
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Core;
    using Xunit;
	using Rhino.Etl.Dsl;

	public class BaseAggregationFixture : BaseDslTest
    {
        protected List<Row> rows;

        public BaseAggregationFixture()
        {
            rows = new List<Row>();
            AddRow("milk", 15);
            AddRow("milk", 15);
            AddRow("sugar", 10);
            AddRow("sugar", 15);
            AddRow("coffee", 6);
            AddRow("sugar", 3);
        }

        private void AddRow(string name, int price)
        {
            Row row = new Row();
            row["name"] = name;
            row["price"] = price;
            rows.Add(row);
        }
    }
}