namespace Rhino.Etl.Core
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Operations;

    /// <summary>
    /// Interface that abastract the actual execution of the pipeline
    /// </summary>
    public interface IPipelineExecuter
    {
        /// <summary>
        /// Executes the specified pipeline.
        /// </summary>
        /// <param name="pipelineName">The name.</param>
        /// <param name="pipeline">The pipeline.</param>
        void Execute(string pipelineName, ICollection<IOperation> pipeline);

		/// <summary>
		/// Transform the pipeline to an enumerable
		/// </summary>
		/// <param name="pipeline">The pipeline.</param>
		/// <param name="rows">The rows.</param>
		/// <returns></returns>
        IEnumerable<Row> PipelineToEnumerable(ICollection<IOperation> pipeline, IEnumerable<Row> rows);

        /// <summary>
        /// Gets all errors that occured under this executer
        /// </summary>
        /// <returns></returns>
        IEnumerable<Exception> GetAllErrors();

        /// <summary>
        /// Gets a value indicating whether this instance has errors.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has errors; otherwise, <c>false</c>.
        /// </value>
        bool HasErrors { get; }
    }
}