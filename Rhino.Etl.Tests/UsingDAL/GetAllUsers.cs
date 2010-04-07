namespace Rhino.Etl.Tests.UsingDAL
{
    using System.Collections.Generic;
    using Core;
    using Rhino.Etl.Core.Operations;

    public class GetAllUsers : AbstractOperation
    {
        public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            foreach (User user in MySimpleDal.GetUsers())
            {
                yield return Row.FromObject(user);
            }
        }
    }
}