namespace Rhino.Etl.Core.ConventionOperations
{
    using System.Data.SqlClient;
    using Operations;

    /// <summary>
    /// Convention class for sql batching.
    /// </summary>
    public class ConventionSqlBatchOperation : SqlBatchOperation
    {
        private string command;

        /// <summary>
        /// Gets or sets the command text to execute
        /// </summary>
        /// <value>The command.</value>
        public string Command
        {
            get { return command; }
            set { command = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionSqlBatchOperation"/> class.
        /// </summary>
        /// <param name="connectionStringName">Name of the connection string.</param>
        public ConventionSqlBatchOperation(string connectionStringName) : base(connectionStringName)
        {
        }

        /// <summary>
        /// Prepares the command.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="sqlCommand">The SQL command.</param>
        protected override void PrepareCommand(Row row, SqlCommand sqlCommand)
        {
            sqlCommand.CommandText = Command;    
            CopyRowValuesToCommandParameters(sqlCommand, row);
        }
    }
}