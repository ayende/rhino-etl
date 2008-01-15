namespace Rhino.Etl.Tests.Aggregation
{
    using System.Collections.Generic;
    using Core;
    using MbUnit.Framework;

    [TestFixture]
    public class AggregationFixture : BaseAggregationFixture
    {
        [Test]
        public void AggregateRowCount()
        {
            using (RowCount rowCount = new RowCount())
            {
                IEnumerable<Row> result = rowCount.Execute(rows);
                List<Row> items = new List<Row>(result);
                Assert.AreEqual(1, items.Count);
                Assert.AreEqual(6, items[0]["count"]);
            }
        }

        [Test]
        public void AggregateCostPerProduct()
        {
            using (CostPerProductAggregation aggregation = new CostPerProductAggregation())
            {
                IEnumerable<Row> result = aggregation.Execute(rows);
                List<Row> items = new List<Row>(result);
                Assert.AreEqual(3, items.Count);
                Assert.AreEqual("milk", items[0]["name"]);
                Assert.AreEqual("sugar", items[1]["name"]);
                Assert.AreEqual("coffee", items[2]["name"]);

                Assert.AreEqual(30, items[0]["cost"]);
                Assert.AreEqual(28, items[1]["cost"]);
                Assert.AreEqual(6, items[2]["cost"]);
            }
        }
    }
}