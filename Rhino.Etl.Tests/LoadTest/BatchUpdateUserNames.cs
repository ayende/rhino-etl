namespace Rhino.Etl.Tests.LoadTest
{
    using System.Data.SqlClient;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class BatchUpdateUserNames : SqlBatchOperation
    {
        public BatchUpdateUserNames()
            : base("test")
        {
        }

        /// <summary>
        /// Prepares the command from the given row
        /// </summary>
        /// <param name="row">The row.</param>
        /// <param name="command">The command.</param>
        protected override void PrepareCommand(Row row, SqlCommand command)
        {
            command.CommandText = "UPDATE Users SET Name = @Name, TestMsg = 'UpperCased' WHERE Id = @Id";
            command.Parameters.AddWithValue("@Name", row["Name"]);
            command.Parameters.AddWithValue("@Id", row["Id"]);
        }
    }
}