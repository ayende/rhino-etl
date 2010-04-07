namespace Rhino.Etl.Tests.UsingDAL
{
    using System.Collections.Generic;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class SaveToDal : AbstractOperation
    {
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (Row row in rows)
            {
                MySimpleDal.Save(row.ToObject<User>());
            }
            yield break;
        }
    }
}