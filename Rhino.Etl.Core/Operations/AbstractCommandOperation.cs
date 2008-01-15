namespace Rhino.Etl.Core.Operations
{
    using System;
    using System.Collections;
    using System.Data;

    /// <summary>
    /// Base class for operations that directly manipulate ADO.Net
    /// It is important to remember that this is supposed to be a deep base class, not to be 
    /// directly inherited or used
    /// </summary>
    public abstract class AbstractCommandOperation : AbstractDatabaseOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractDatabaseOperation"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        protected AbstractCommandOperation(string connectionStringName) : base(connectionStringName)
        {
        }

        /// <summary>
        /// The current command
        /// </summary>
        protected IDbCommand currentCommand;

		/// <summary>
		/// Adds the parameter to the current command
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="value">The value.</param>
		protected void AddParameter(string name, object value)
		{
			AddParameter(currentCommand, name, value);
		}
    }
}