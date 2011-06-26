using System;
using System.Collections.Generic;

namespace Rhino.Etl.Core.Operations
{
	/// <summary>
	/// Branch the current pipeline flow into all its inputs
	/// </summary>
	public abstract class AbstractBranchingOperation : AbstractOperation
	{
		/// <summary>
		/// Creates a new <see cref="AbstractOperation"/>
		/// </summary>
		protected AbstractBranchingOperation()
		{
			Operations = new List<IOperation>();
		}

		/// <summary>
		/// Returns the list of child operations
		/// </summary>
		protected List<IOperation> Operations { get; private set; }

		/// <summary>
		/// Adds the specified operation to this branching operation
		/// </summary>
		/// <param name="operation">The operation.</param>
		/// <returns></returns>
		public AbstractBranchingOperation Add(IOperation operation)
		{
			Operations.Add(operation);
			return this;
		}

		/// <summary>
		/// Initializes this instance
		/// </summary>
		/// <param name="pipelineExecuter">The current pipeline executer.</param>
		public override void PrepareForExecution(IPipelineExecuter pipelineExecuter)
		{
			base.PrepareForExecution(pipelineExecuter);
			foreach (IOperation operation in Operations)
			{
				operation.PrepareForExecution(pipelineExecuter);
			}
		}

		///	<summary>
		///	Occurs when	a row is processed.
		///	</summary>
		public override	event Action<IOperation, Row> OnRowProcessed
		{
			add
			{
				foreach	(IOperation	operation in Operations)
					operation.OnRowProcessed +=	value;
				base.OnRowProcessed	+= value;
			}
			remove
			{
				foreach	(IOperation	operation in Operations)
					operation.OnRowProcessed -=	value;
				base.OnRowProcessed	-= value;
			}
		}

		///	<summary>
		///	Occurs when	all	the	rows has finished processing.
		///	</summary>
		public override	event Action<IOperation> OnFinishedProcessing
		{
			add
			{
				foreach	(IOperation	operation in Operations)
					operation.OnFinishedProcessing += value;
				base.OnFinishedProcessing += value;
			}
			remove
			{
				foreach	(IOperation	operation in Operations)
					operation.OnFinishedProcessing -= value;
				base.OnFinishedProcessing -= value;
			}
		}
	}
}