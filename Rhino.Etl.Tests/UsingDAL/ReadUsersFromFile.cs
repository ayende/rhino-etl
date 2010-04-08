namespace Rhino.Etl.Tests.UsingDAL
{
    using System.Collections.Generic;
    using Core;
    using Rhino.Etl.Core.Files;
    using Rhino.Etl.Core.Operations;

    public class ReadUsersFromFile : AbstractOperation
    {
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            using(FileEngine file = FluentFile.For<UserRecord>().From("users.txt"))
            {
                foreach (object obj in file)
                {
                    yield return Row.FromObject(obj);
                }
            }
        }
    }
}