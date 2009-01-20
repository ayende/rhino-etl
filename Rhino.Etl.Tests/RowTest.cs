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
        [Row(new[] {"a"}, new object[] {null}, new[] {"a"}, new object[] {1})]
        [Row(new[] {"a"}, new object[] {1}, new[] {"a"}, new object[] {null})]
        public void Should_be_different_if_different_columns(string[] firstColumns, object[] firstValues,
                                                             string[] secondColumns, object[] secondValues)
        {
            Row first = FillRow(firstColumns, firstValues);
            Row second = FillRow(secondColumns, secondValues);

            Assert.IsFalse(first.Equals(second));
        }

        [Test]
        public void Nulls_are_equal()
        {
            Row first = new Row();
            first["a"] = null;

            Row second = new Row();
            second["a"] = null;

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void Should_not_take_casing_into_account()
        {
            Row first = new Row();
            first["A"] = 1;

            Row second = new Row();
            second["a"] = 1;

            Assert.IsTrue(first.Equals(second));
        }

        [Test]
        public void Since_values_are_boxed_they_wont_be_equal_even_if_implicit_conversion_exists()
        {
            Row first = new Row();
            first["a"] = 1;

            Row second = new Row();
            second["a"] = (byte)1;

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
