namespace Rhino.Etl.Tests.LoadTest
{
    using Rhino.Etl.Core.ConventionOperations;

    public class ReadUsers : ConventionInputCommandOperation
    {
        public ReadUsers() : base("test")
        {
            Command = "SELECT Id, Name,Email FROM Users";
        }
    }
}