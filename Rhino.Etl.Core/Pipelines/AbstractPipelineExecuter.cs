namespace Rhino.Etl.Core.Pipelines
{
    using System;
    using System.Collections.Generic;
    using Operations;

    /// <summary>
    /// Base class for pipeline executers, handles all the details and leave the actual
    /// pipeline execution to the 
    /// </summary>
    public abstract class AbstractPipelineExecuter : WithLoggingMixin, IPipelineExecuter
    {
        #region IPipelineExecuter Members

        /// <summary>
        /// Executes the specified pipeline.
        /// </summary>
        /// <param name="pipelineName">The name.</param>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="translateRows">Translate the rows into another representation</param>
        public void Execute(string pipelineName,
                            ICollection<IOperation> pipeline,
                            Func<IEnumerable<Row>, IEnumerable<Row>> translateRows)
        {
            try
            {
                IEnumerable<Row> enumerablePipeline = PipelineToEnumerable(pipeline, new List<Row>(), translateRows);
                try
                {
                    raiseNotifyExecutionStarting();
                    DateTime start = DateTime.Now;
                    ExecutePipeline(enumerablePipeline);
                    raiseNotifyExecutionCompleting();
                    Trace("Completed process {0} in {1}", pipelineName, DateTime.Now - start);
                }
                catch (Exception e)
                {
                    string errorMessage = string.Format("Failed to execute pipeline {0}", pipelineName);
                    Error(e, errorMessage);
                }
            }
            catch (Exception e)
            {
                Error(e, "Failed to create pipeline {0}", pipelineName);                
            }

            DisposeAllOperations(pipeline);
        }

        /// <summary>
        /// Transform the pipeline to an enumerable
        /// </summary>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="rows">The rows</param>
        /// <param name="translateEnumerable">Translate the rows from one representation to another</param>
        /// <returns></returns>
        public virtual IEnumerable<Row> PipelineToEnumerable(
            ICollection<IOperation> pipeline, 
            IEnumerable<Row> rows,
            Func<IEnumerable<Row>, IEnumerable<Row>> translateEnumerable)
        {
            foreach (var operation in pipeline)
            {
                operation.PrepareForExecution(this);
                var enumerator = operation.Execute(rows);
                enumerator = translateEnumerable(enumerator);
                rows = DecorateEnumerableForExecution(operation, enumerator);
            }
            return rows;
        }

        /// <summary>
        /// Gets all errors that occured under this executer
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Exception> GetAllErrors()
        {
            return Errors;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        public bool HasErrors
        {
            get { return Errors.Length != 0; }
        }

        #endregion

        /// <summary>
        /// Iterates the specified enumerable.
        /// Since we use a pipeline, we need to force it to execute at some point. 
        /// We aren't really interested in the result, just in that the pipeline would execute.
        /// </summary>
        protected virtual void ExecutePipeline(IEnumerable<Row> pipeline)
        {
            var enumerator = pipeline.GetEnumerator();
            try
            {
#pragma warning disable 642
                while (enumerator.MoveNext()) ;
#pragma warning restore 642
            }
            catch (Exception e)
            {
                Error(e, "Failed to execute operation {0}", enumerator.Current);
            }
        }


        /// <summary>
        /// Destroys the pipeline.
        /// </summary>
        protected void DisposeAllOperations(ICollection<IOperation> operations)
        {
            foreach (IOperation operation in operations)
            {
                try
                {
                    operation.Dispose();
                }
                catch (Exception e)
                {
                    Error(e, "Failed to disposed {0}", operation.Name);
                }
            }
        }

        ///    <summary>
        ///    Occurs when    the    pipeline has been successfully created,    but    before it is executed
        ///    </summary>
        public event Action<IPipelineExecuter> NotifyExecutionStarting = delegate {    };

        ///    <summary>
        ///    Raises the ExecutionStarting event
        ///    </summary>
        private    void raiseNotifyExecutionStarting()
        {
            NotifyExecutionStarting(this);
        }

        ///    <summary>
        ///    Occurs when    the    pipeline has been successfully created,    but    before it is disposed
        ///    </summary>
        public event Action<IPipelineExecuter> NotifyExecutionCompleting = delegate    { };

        ///    <summary>
        ///    Raises the ExecutionCompleting event
        ///    </summary>
        private    void raiseNotifyExecutionCompleting()
        {
            NotifyExecutionCompleting(this);
        }

        ///    <summary>
        /// Add a decorator to the enumerable for additional processing
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="enumerator">The enumerator.</param>
        protected abstract IEnumerable<Row> DecorateEnumerableForExecution(IOperation operation, IEnumerable<Row> enumerator);
    }
}