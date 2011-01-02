namespace Rhino.Etl.Core
{
    using System.Collections.Generic;
    using Operations;

    /// <summary>
    /// Base class for etl processes, provider registration and management
    /// services for the pipeline
    /// </summary>
    /// <typeparam name="TDerived">The type of the derived.</typeparam>
    public class EtlProcessBase<TDerived> : WithLoggingMixin 
        where TDerived : EtlProcessBase<TDerived>
    {
        /// <summary>
        /// Internal field to indicate if a transaction is used. Defaulting to true.
        /// </summary>
        private bool useTransaction = true;

        /// <summary>
        /// Ordered list of the operations in this process that will be added to the
        /// operations list after the initialization is completed.
        /// </summary>
        private readonly List<IOperation> lastOperations = new List<IOperation>();

        /// <summary>
        /// Ordered list of the operations in this process
        /// </summary>
        protected readonly List<IOperation> operations = new List<IOperation>();

        /// <summary>
        /// Gets the name of this instance
        /// </summary>
        /// <value>The name.</value>
        public virtual string Name
        {
            get { return GetType().Name; }
        }

        /// <summary>
        /// Gets or sets whether we are using a transaction
        /// </summary>
        /// <value>True or value.</value>
        public bool UseTransaction
        {
            get { return useTransaction; }
            set { useTransaction = value; }
        }

        /// <summary>
        /// Registers the specified operation.
        /// </summary>
        /// <param name="operation">The operation.</param>
        public TDerived Register(IOperation operation)
        {
            operation.UseTransaction = UseTransaction;
            operations.Add(operation);
            Debug("Register {0} in {1}", operation.Name, Name);
            return (TDerived) this;
        }

        /// <summary>
        /// Registers the operation at the end of the operations queue
        /// </summary>
        /// <param name="operation">The operation.</param>
        public TDerived RegisterLast(IOperation operation)
        {
            lastOperations.Add(operation);
            Debug("RegisterLast {0} in {1}", operation.Name, Name);
            return (TDerived) this;
        }

        /// <summary>
        /// Merges the last operations to the operations list.
        /// </summary>
        protected void MergeLastOperationsToOperations()
        {
            operations.AddRange(lastOperations);
        }
    }
}