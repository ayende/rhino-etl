namespace Rhino.Etl.Core.Operations
{
	using System;
	using System.Collections.Generic;
	using Commons;
	using Enumerables;

	/// <summary>
	/// Perform a join between two sources
	/// </summary>
	public abstract class JoinOperation : AbstractOperation
	{
		private readonly PartialProcessOperation left = new PartialProcessOperation();
		private readonly PartialProcessOperation right = new PartialProcessOperation();
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
			return this;
		}

		/// <summary>
		/// Executes this operation
		/// </summary>
		/// <param name="ignored">Ignored rows</param>
		/// <returns></returns>
		public override IEnumerable<Row> Execute(IEnumerable<Row> ignored)
		{
			PrepareForJoin();

			IEnumerable<Row> rightEnumerable = GetRightEnumerable();

			IEnumerable<Row> execute = left.Execute(null);
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

		private void PrepareForJoin()
		{
			Initialize();

			Guard.Against(left == null, "Left branch of a join cannot be null");
			Guard.Against(right == null, "Right branch of a join cannot be null");

			SetupJoinConditions();

			Guard.Against(leftColumns == null, "You must setup the left columns");
			Guard.Against(rightColumns == null, "You must setup the right columns");
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
		/// Called when a row on the right side was filtered by
		/// the join condition, allow a derived class to perform 
		/// logic associated to that, such as logging
		/// </summary>
		protected virtual void RightOrphanRow(Row row)
		{

		}

		/// <summary>
		/// Called when a row on the left side was filtered by
		/// the join condition, allow a derived class to perform 
		/// logic associated to that, such as logging
		/// </summary>
		/// <param name="row">The row.</param>
		protected virtual void LeftOrphanRow(Row row)
		{

		}

		/// <summary>
		/// Merges the two rows into a single row
		/// </summary>
		/// <param name="leftRow">The left row.</param>
		/// <param name="rightRow">The right row.</param>
		/// <returns></returns>
		protected abstract Row MergeRows(Row leftRow, Row rightRow);

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		protected virtual void Initialize()
		{
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
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public override void Dispose()
		{
			left.Dispose();
			right.Dispose();
		}


		/// <summary>
		/// Initializes this instance
		/// </summary>
		/// <param name="pipelineExecuter">The current pipeline executer.</param>
		public override void PrepareForExecution(IPipelineExecuter pipelineExecuter)
		{
			left.PrepareForExecution(pipelineExecuter);
			right.PrepareForExecution(pipelineExecuter);
		}

		/// <summary>
		/// Gets all errors that occured when running this operation
		/// </summary>
		/// <returns></returns>
		public override IEnumerable<Exception> GetAllErrors()
		{
			foreach (Exception error in left.GetAllErrors())
			{
				yield return error;
			}
			foreach (Exception error in right.GetAllErrors())
			{
				yield return error;
			}
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
	}
}