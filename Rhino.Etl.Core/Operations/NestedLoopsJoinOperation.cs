namespace Rhino.Etl.Core.Operations
{
	using System;
	using System.Collections.Generic;
	using Commons;
	using Enumerables;

	/// <summary>
	/// Perform a join between two sources
	/// </summary>
	public abstract class NestedLoopsJoinOperation : AbstractOperation
	{
		private readonly PartialProcessOperation left = new PartialProcessOperation();
		private readonly PartialProcessOperation right = new PartialProcessOperation();
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
			return this;
		}

		/// <summary>
		/// Executes this operation
		/// </summary>
		/// <param name="ignored">Ignored rows</param>
		/// <returns></returns>
		public override IEnumerable<Row> Execute(IEnumerable<Row> ignored)
		{
			Initialize();

			Guard.Against(left == null, "Left branch of a join cannot be null");
			Guard.Against(right == null, "Right branch of a join cannot be null");

			Dictionary<Row, object> matchedRightRows = new Dictionary<Row, object>();
			CachingEnumerable<Row> rightEnumerable = new CachingEnumerable<Row>(
				new EventRaisingEnumerator(right, right.Execute(null))
				);
			IEnumerable<Row> execute = left.Execute(null);
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
		/// Check if the two rows match to the join condition.
		/// </summary>
		/// <param name="leftRow">The left row.</param>
		/// <param name="rightRow">The right row.</param>
		/// <returns></returns>
		protected abstract bool MatchJoinCondition(Row leftRow, Row rightRow);

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		protected virtual void Initialize()
		{
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
		/// Perform an inner join equality on the two objects.
		/// Null values are not considered equal
		/// </summary>
		/// <param name="left">The left.</param>
		/// <param name="right">The right.</param>
		/// <returns></returns>
		protected virtual bool InnerJoin(object left, object right)
		{
			if(IsEmptyRow(currentLeftRow) || IsEmptyRow(currentRightRow))
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
			if(IsEmptyRow(currentRightRow))
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
			if(IsEmptyRow(currentLeftRow))
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
			if(IsEmptyRow(currentLeftRow) || IsEmptyRow(currentRightRow))
				return true;
			if (left == null || right == null)
				return false;
			return Equals(left, right);
		}
	}
}