namespace Rhino.Etl.Core.Operations
{
    using Enumerables;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Perform a join between two sources. The left part of the join is optional and if not specified it will use the current pipeline as input.
    /// </summary>
    public abstract class NestedLoopsJoinOperation : AbstractJoinOperation
    {
        private static readonly string IsEmptyRowMarker = Guid.NewGuid().ToString();

        private Row currentRightRow, currentLeftRow;

        /// <summary>
        /// Sets the right part of the join
        /// </summary>
        /// <value>The right.</value>
        public NestedLoopsJoinOperation Right(IOperation value)
        {
            right.Register(value);
            return this;
        }

        /// <summary>
        /// Sets the left part of the join
        /// </summary>
        /// <value>The left.</value>
        public NestedLoopsJoinOperation Left(IOperation value)
        {
            left.Register(value);
            leftRegistered = true;
            return this;
        }

        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows">Rows in pipeline. These are only used if a left part of the join was not specified.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            PrepareForJoin();

            Dictionary<Row, object> matchedRightRows = new Dictionary<Row, object>();
            CachingEnumerable<Row> rightEnumerable = new CachingEnumerable<Row>(
                new EventRaisingEnumerator(right, right.Execute(null))
                );
            IEnumerable<Row> execute = left.Execute(leftRegistered ? null : rows);
            foreach (Row leftRow in new EventRaisingEnumerator(left, execute))
            {
                bool leftNeedOuterJoin = true;
                currentLeftRow = leftRow;
                foreach (Row rightRow in rightEnumerable)
                {
                    currentRightRow = rightRow;
                    if (MatchJoinCondition(leftRow, rightRow))
                    {
                        leftNeedOuterJoin = false;
                        matchedRightRows[rightRow] = null;
                        yield return MergeRows(leftRow, rightRow);
                    }
                }
                if (leftNeedOuterJoin)
                {
                    Row emptyRow = new Row();
                    emptyRow[IsEmptyRowMarker] = IsEmptyRowMarker;
                    currentRightRow = emptyRow;
                    if (MatchJoinCondition(leftRow, emptyRow))
                        yield return MergeRows(leftRow, emptyRow);
                    else
                        LeftOrphanRow(leftRow);
                }
            }
            foreach (Row rightRow in rightEnumerable)
            {
                if (matchedRightRows.ContainsKey(rightRow))
                    continue;
                currentRightRow = rightRow;
                Row emptyRow = new Row();
                emptyRow[IsEmptyRowMarker] = IsEmptyRowMarker;
                currentLeftRow = emptyRow;
                if (MatchJoinCondition(emptyRow, rightRow))
                    yield return MergeRows(emptyRow, rightRow);
                else
                    RightOrphanRow(rightRow);
            }
        }

        /// <summary>
        /// Check if the two rows match to the join condition.
        /// </summary>
        /// <param name="leftRow">The left row.</param>
        /// <param name="rightRow">The right row.</param>
        /// <returns></returns>
        protected abstract bool MatchJoinCondition(Row leftRow, Row rightRow);

        /// <summary>
        /// Perform an inner join equality on the two objects.
        /// Null values are not considered equal
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        protected virtual bool InnerJoin(object left, object right)
        {
            if (IsEmptyRow(currentLeftRow) || IsEmptyRow(currentRightRow))
                return false;
            if (left == null || right == null)
                return false;
            return left.Equals(right);
        }

        private static bool IsEmptyRow(Row row)
        {
            return row.Contains(IsEmptyRowMarker);
        }

        /// <summary>
        /// Perform an left join equality on the two objects.
        /// Null values are not considered equal
        /// An empty row on the right side
        /// with a value on the left is considered equal
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        protected virtual bool LeftJoin(object left, object right)
        {
            if (IsEmptyRow(currentRightRow))
                return true;
            if (left == null || right == null)
                return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Perform an right join equality on the two objects.
        /// Null values are not considered equal
        /// An empty row on the left side
        /// with a value on the right is considered equal
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        protected virtual bool RightJoin(object left, object right)
        {
            if (IsEmptyRow(currentLeftRow))
                return true;
            if (left == null || right == null)
                return false;
            return left.Equals(right);
        }

        /// <summary>
        /// Perform an full join equality on the two objects.
        /// Null values are not considered equal
        /// An empty row on either side will satisfy this join
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns></returns>
        protected virtual bool FullJoin(object left, object right)
        {
            if (IsEmptyRow(currentLeftRow) || IsEmptyRow(currentRightRow))
                return true;
            if (left == null || right == null)
                return false;
            return Equals(left, right);
        }
    }
}