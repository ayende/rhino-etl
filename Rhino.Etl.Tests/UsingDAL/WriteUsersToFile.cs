namespace Rhino.Etl.Tests.UsingDAL
{
    using System.Collections.Generic;
    using Core;
    using FileHelpers;
    using Rhino.Etl.Core.Files;
    using Rhino.Etl.Core.Operations;

    public class WriteUsersToFile : AbstractOperation
    {
        /// <summary>
        /// Executes this operation
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <returns></returns>
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            FluentFile engine = FluentFile.For<UserRecord>();
            engine.HeaderText = "Id\tName\tEmail";
            using(FileEngine file = engine.To("users.txt"))
            {
               
                foreach (Row row in rows)
                {
                    UserRecord record = new UserRecord();
                    
                    record.Id = (int)row["id"];
                    record.Name = (string)row["name"];
                    record.Email = (string)row["email"];

                    file.Write(record);
                }
            }
            yield break;
        }
    }
}