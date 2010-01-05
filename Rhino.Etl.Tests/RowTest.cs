using System;
using Xunit;
using Rhino.Etl.Core;
using Xunit.Extensions;

namespace Rhino.Etl.Tests
{
    
    public class RowTest
    {
        [Theory]
        [InlineData(new[] { "a", "b" }, new object[] { 1, 2 }, new[] { "a", "b" }, new object[] { 2, 3 })]
        [InlineData(new[] { "a", "b" }, new object[] { 1, 2 }, new[] { "x", "y" }, new object[] { 1, 2 })]
        [InlineData(new[] { "a" }, new object[] { 1 }, new[] { "a", "b" }, new object[] { 1, 2 })]
        [InlineData(new[] { "a" }, new object[] { null }, new[] { "a" }, new object[] { 1 })]
        [InlineData(new[] { "a" }, new object[] { 1 }, new[] { "a" }, new object[] { null })]
        public void Should_be_different_if_different_columns(string[] firstColumns, object[] firstValues,
                                                             string[] secondColumns, object[] secondValues)
        {
            Row first = FillRow(firstColumns, firstValues);
            Row second = FillRow(secondColumns, secondValues);

            Assert.False(first.Equals(second));
        }

        [Fact]
        public void Nulls_are_equal()
        {
            Row first = new Row();
            first["a"] = null;

            Row second = new Row();
            second["a"] = null;

            Assert.True(first.Equals(second));
        }

        [Fact]
        public void Should_not_take_casing_into_account_in_column_names()
        {
            Row first = new Row();
            first["A"] = 1;

            Row second = new Row();
            second["a"] = 1;

            Assert.True(first.Equals(second));
        }

        [Theory]
        [InlineData((byte)1, (int)1)]
        [InlineData((int)1, (long)1)]
        [InlineData((long)1, (float)1)]
        [InlineData((float)1, (double)1)]
        public void Different_numeric_types_should_be_comparable_if_implicit_conversion_exists(object firstValue, object secondValue)
        {
            Row first = new Row();
            first["a"] = firstValue;

            Row second = new Row();
            second["a"] = secondValue;

            Assert.True(first.Equals(second));
        }

        [Fact]
        public void Incompatible_types_throw_invalid_operation_exception()
        {
            Row first = new Row();
            first["a"] = 1;

            Row second = new Row();
            second["a"] = "stringvalue";

            Assert.Throws<InvalidOperationException>(() => first.Equals(second));
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
