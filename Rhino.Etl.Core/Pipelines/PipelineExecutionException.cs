using System;

namespace Rhino.Etl.Core.Pipelines
{
    /// <summary>
    /// Exception class for when a pipeline throws an execution error.
    /// </summary>
    public class PipelineExecutionException : Exception
    {
        internal PipelineExecutionException(string Message, Exception InnerException)
            : base(Message, InnerException)
        {
        }
    }
}