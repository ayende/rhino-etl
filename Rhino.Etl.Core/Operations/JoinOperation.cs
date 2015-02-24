namespace Rhino.Etl.Core.Operations
{
    using Enumerables;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Perform a join between two sources. The left part of the join is optional and if not specified it will use the current pipeline as input.
    /// </summary>
    public abstract class JoinOperation : AbstractJoinOperation
    {
        private JoinType jointype;
        private string[] leftColumns;
        private string[] rightColumns;
        private Dictionary<Row, object> rightRowsWereMatched = new Dictionary<Row, object>();
        private Dictionary<ObjectArrayKeys, List<Row>> rightRowsByJoinKey = new Dictionary<ObjectArrayKeys, List<Row>>();

        /// <summary>
        /// Sets the right part of the join
        /// </summary>
        /// <value>The right.</value>
        public JoinOperation Right(IOperation value)
        {
            right.Register(value);
            return this;
        }

        /// <summary>
        /// Sets the left part of the join
        /// </summary>
        /// <value>The left.</value>
        public JoinOperation Left(IOperation value)
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

            SetupJoinConditions();
            Guard.Against(leftColumns == null, "You must setup the left columns");
            Guard.Against(rightColumns == null, "You must setup the right columns");

            IEnumerable<Row> rightEnumerable = GetRightEnumerable();

            IEnumerable<Row> execute = left.Execute(leftRegistered ? null : rows);
            foreach (Row leftRow in new EventRaisingEnumerator(left, execute))
            {
                ObjectArrayKeys key = leftRow.CreateKey(leftColumns);
                List<Row> rightRows;
                if (this.rightRowsByJoinKey.TryGetValue(key, out rightRows))
                {
                    foreach (Row rightRow in rightRows)
                    {
                        rightRowsWereMatched[rightRow] = null;
                        yield return MergeRows(leftRow, rightRow);
                    }
                }
                else if ((jointype & JoinType.Left) != 0)
                {
                    Row emptyRow = new Row();
                    yield return MergeRows(leftRow, emptyRow);
                }
                else
                {
                    LeftOrphanRow(leftRow);
                }
            }
            foreach (Row rightRow in rightEnumerable)
            {
                if (rightRowsWereMatched.ContainsKey(rightRow))
                    continue;
                Row emptyRow = new Row();
                if ((jointype & JoinType.Right) != 0)
                    yield return MergeRows(emptyRow, rightRow);
                else
                    RightOrphanRow(rightRow);
            }
        }

        private IEnumerable<Row> GetRightEnumerable()
        {
            IEnumerable<Row> rightEnumerable = new CachingEnumerable<Row>(
                new EventRaisingEnumerator(right, right.Execute(null))
                );
            foreach (Row row in rightEnumerable)
            {
                ObjectArrayKeys key = row.CreateKey(rightColumns);
                List<Row> rowsForKey;
                if (this.rightRowsByJoinKey.TryGetValue(key, out rowsForKey) == false)
                {
                    this.rightRowsByJoinKey[key] = rowsForKey = new List<Row>();
                }
                rowsForKey.Add(row);
            }
            return rightEnumerable;
        }

        /// <summary>
        /// Setups the join conditions.
        /// </summary>
        protected abstract void SetupJoinConditions();

        /// <summary>
        /// Create an inner join
        /// </summary>
        /// <value>The inner.</value>
        protected JoinBuilder InnerJoin
        {
            get { return new JoinBuilder(this, JoinType.Inner); }
        }

        /// <summary>
        /// Create a left outer join
        /// </summary>
        /// <value>The inner.</value>
        protected JoinBuilder LeftJoin
        {
            get { return new JoinBuilder(this, JoinType.Left); }
        }

        /// <summary>
        /// Create a right outer join
        /// </summary>
        /// <value>The inner.</value>
        protected JoinBuilder RightJoin
        {
            get { return new JoinBuilder(this, JoinType.Right); }
        }

        /// <summary>
        /// Create a full outer join
        /// </summary>
        /// <value>The inner.</value>
        protected JoinBuilder FullOuterJoin
        {
            get { return new JoinBuilder(this, JoinType.Full); }
        }

        /// <summary>
        /// Fluent interface to create joins
        /// </summary>
        public class JoinBuilder
        {
            private readonly JoinOperation parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="JoinBuilder"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            /// <param name="joinType">Type of the join.</param>
            public JoinBuilder(JoinOperation parent, JoinType joinType)
            {
                this.parent = parent;
                parent.jointype = joinType;
            }

            /// <summary>
            /// Setup the left side of the join
            /// </summary>
            /// <param name="columns">The columns.</param>
            /// <returns></returns>
            public JoinBuilder Left(params string[] columns)
            {
                parent.leftColumns = columns;
                return this;
            }

            /// <summary>
            /// Setup the right side of the join
            /// </summary>
            /// <param name="columns">The columns.</param>
            /// <returns></returns>
            public JoinBuilder Right(params string[] columns)
            {
                parent.rightColumns = columns;
                return this;
            }
        }

        ///    <summary>
        ///    Occurs when    a row is processed.
        ///    </summary>
        public override event Action<IOperation, Row> OnRowProcessed
        {
            add
            {
                foreach (IOperation operation in new[] { left, right })
                    operation.OnRowProcessed += value;
                base.OnRowProcessed += value;
            }
            remove
            {
                foreach (IOperation operation in new[] { left, right })
                    operation.OnRowProcessed -= value;
                base.OnRowProcessed -= value;
            }
        }

        ///    <summary>
        ///    Occurs when    all    the    rows has finished processing.
        ///    </summary>
        public override event Action<IOperation> OnFinishedProcessing
        {
            add
            {
                foreach (IOperation operation in new[] { left, right })
                    operation.OnFinishedProcessing += value;
                base.OnFinishedProcessing += value;
            }
            remove
            {
                foreach (IOperation operation in new[] { left, right })
                    operation.OnFinishedProcessing -= value;
                base.OnFinishedProcessing -= value;
            }
        }
    }
}