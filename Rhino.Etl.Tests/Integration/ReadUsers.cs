namespace Rhino.Etl.Tests.Integration
{
    using System.Data;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class ReadUsers : InputCommandOperation
    {
        public ReadUsers() : base("test")
        {
        }

        protected override Row CreateRowFromReader(IDataReader reader)
        {
            return Row.FromReader(reader);
        }

        protected override void PrepareCommand(IDbCommand cmd)
        {
            cmd.CommandText = "SELECT * FROM Users";
        }
    }
}