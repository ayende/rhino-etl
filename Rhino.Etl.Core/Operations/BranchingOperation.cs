using System.Linq;
using Rhino.Etl.Core.Enumerables;

namespace Rhino.Etl.Core.Operations
{
    using System.Collections.Generic;

    /// <summary>
	/// Branch the current pipeline flow into all its inputs
	/// </summary>
	public class BranchingOperation : AbstractOperation
	{
		private readonly List<IOperation> operations = new List<IOperation>();

		/// <summary>
		/// Adds the specified operation to this branching operation
		/// </summary>
		/// <param name="operation">The operation.</param>
		/// <returns></returns>
		public BranchingOperation Add(IOperation operation)
		{
			operations.Add(operation);
			return this;
		}

		/// <summary>
		/// Initializes this instance
		/// </summary>
		/// <param name="pipelineExecuter">The current pipeline executer.</param>
		public override void PrepareForExecution(IPipelineExecuter pipelineExecuter)
		{
			base.PrepareForExecution(pipelineExecuter);
			foreach (IOperation operation in operations)
			{
				operation.PrepareForExecution(pipelineExecuter);
			}
		}

		/// <summary>
		/// Executes this operation, sending the input of this operation
		/// to all its child operations
		/// </summary>
		/// <param name="rows">The rows.</param>
		/// <returns></returns>
		public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
		{
		    var copiedRows = new CachingEnumerable<Row>(rows);

			foreach (IOperation operation in operations)
			{
                var cloned = copiedRows.Select(r => r.Clone());

				IEnumerable<Row> enumerable = operation.Execute(cloned);

				if(enumerable==null)
					continue;

				IEnumerator<Row> enumerator = enumerable.GetEnumerator();
#pragma warning disable 642
				while (enumerator.MoveNext()) ;
#pragma warning restore 642
			}
			yield break;
		}
	}
}