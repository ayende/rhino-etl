namespace Rhino.Etl.Core.Operations
{
    using System;
    using System.Threading;

    /// <summary>
    /// Contains the statistics for an operation
    /// </summary>
    public class OperationStatistics
    {
        private DateTime? start;
        private DateTime? end;
        private long outputtedRows = 0;

        /// <summary>
        /// Gets number of the outputted rows.
        /// </summary>
        /// <value>The processed rows.</value>
        public long OutputtedRows
        {
            get { return outputtedRows; }
        }

        /// <summary>
        /// Gets the duration this operation has executed
        /// </summary>
        /// <value>The duration.</value>
        public TimeSpan Duration
        {
            get
            {
                if( start == null || end == null)
                    return new TimeSpan();

                return end.Value - start.Value;
            }
        }

        /// <summary>
        /// Mark the start time
        /// </summary>
        public void MarkStarted()
        {
            start = DateTime.Now;
        }

        /// <summary>
        /// Mark the end time
        /// </summary>
        public void MarkFinished()
        {
            end = DateTime.Now;
        }

        /// <summary>
        /// Marks a processed row.
        /// </summary>
        public void MarkRowProcessed()
        {
            Interlocked.Increment(ref outputtedRows);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return OutputtedRows + " Rows in " + Duration;
        }

		/// <summary>
		/// Adds to the count of the output rows.
		/// </summary>
		/// <param name="rowProcessed">The row processed.</param>
    	public void AddOutputRows(long rowProcessed)
    	{
    		Interlocked.Increment(ref outputtedRows);
    	}
    }
}