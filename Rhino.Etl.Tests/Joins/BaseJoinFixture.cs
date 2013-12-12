namespace Rhino.Etl.Tests.Joins
{
    using System.Collections.Generic;
    using Core;
    using Xunit;

    public class BaseJoinFixture
    {
        protected List<Row> left;
        protected List<Row> right;

        public BaseJoinFixture()
        {
            left = new List<Row>();
            right = new List<Row>();

            AddUser("foo", "foo@example.org");
            AddUser("bar", "bar@example.org");

            AddPerson(3, "foo@example.org");
            AddPerson(5, "silver@exaple.org");
        }

        protected void AddPerson(int id, string email)
        {
            Row row = new Row();
            row["id"] = id;
            row["email"] = email;
            right.Add(row);
        }

        protected void AddUser(string name, string email)
        {
            Row row = new Row();
            row["name"] = name;
            row["email"] = email;
            left.Add(row);
        }
    }
}