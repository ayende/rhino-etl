using MbUnit.Framework;
using Rhino.Etl.Core;

namespace Rhino.Etl.Tests
{
    [TestFixture]
    public class RowTest
    {
        [RowTest]
        [Row(new[] {"a", "b"}, new object[] {1, 2}, new[] {"a", "b"}, new object[] {2, 3})]
        [Row(new[] {"a", "b"}, new object[] {1, 2}, new[] {"x", "y"}, new object[] {1, 2})]
        [Row(new[] {"a"}, new object[] {1}, new[] {"a", "b"}, new object[] {1, 2})]
        [Row(new[] {"a"}, new object[] {1}, new[] {"a", "b"}, new object[] {1, 2})]
        public void Should_be_different_if_different_columns(string[] firstColumns, object[] firstValues,
                                                             string[] secondColumns, object[] secondValues)
        {
            Row first = FillRow(firstColumns, firstValues);
            Row second = FillRow(secondColumns, secondValues);

            Assert.IsFalse(first.Equals(second));
        }

        private static Row FillRow(string[] columns, object[] values)
        {
            Row row = new Row();

            for (int i = 0; i < values.Length; i++)
                row[columns[i]] = values[i];

            return row;
        }
    }
}
