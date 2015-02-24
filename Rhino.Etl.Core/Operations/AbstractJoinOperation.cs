using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rhino.Etl.Core.Operations
{
    /// <summary>
    /// Perform a join between two sources.
    /// </summary>
    public abstract class AbstractJoinOperation : AbstractOperation
    {
        /// <summary>
        /// The left process
        /// </summary>
        protected readonly PartialProcessOperation left = new PartialProcessOperation();

        /// <summary>
        /// The rigth process
        /// </summary>
        protected readonly PartialProcessOperation right = new PartialProcessOperation();

        /// <summary>
        /// Is left registered?
        /// </summary>
        protected bool leftRegistered = false;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        protected virtual void Initialize()
        {
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
        /// Check left/right branches are not null
        /// </summary>
        protected void PrepareForJoin()
        {
            Initialize();
            Guard.Against(left == null, "Left branch of a join cannot be null");
            Guard.Against(right == null, "Right branch of a join cannot be null");
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
            foreach (Exception error in Errors)
            {
                yield return error;
            }
        }

        /// <summary>
        /// Merges the two rows into a single row
        /// </summary>
        /// <param name="leftRow">The left row.</param>
        /// <param name="rightRow">The right row.</param>
        /// <returns></returns>
        protected abstract Row MergeRows(Row leftRow, Row rightRow);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            left.Dispose();
            right.Dispose();
        }
    }
}