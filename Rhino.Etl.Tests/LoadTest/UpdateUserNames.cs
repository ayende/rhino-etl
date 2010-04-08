namespace Rhino.Etl.Tests.LoadTest
{
    using System.Data.SqlClient;
    using Core;
    using Rhino.Etl.Core.ConventionOperations;
    using Rhino.Etl.Core.Operations;

    public class UpdateUserNames : ConventionOutputCommandOperation
    {
        public UpdateUserNames()
            : base("test")
        {
            Command = "UPDATE Users SET Name = @Name, TestMsg = 'UpperCased' WHERE Id = @Id";
        }
    }
}