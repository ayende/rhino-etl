namespace Rhino.Etl.Tests.Integration
{
    using System.Data;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class WritePeople : OutputCommandOperation
    {
        public WritePeople() : base("test")
        {
        }

        protected override void PrepareCommand(IDbCommand cmd, Row row)
        {
            cmd.CommandText =
                @"INSERT INTO People (UserId, FirstName, LastName, Email) VALUES (@UserId, @FirstName, @LastName, @Email)";
            AddParameter("UserId", row["Id"]);
            AddParameter("FirstName", row["FirstName"]);
            AddParameter("LastName", row["LastName"]);
            AddParameter("Email", row["Email"]);
        }
    }
}