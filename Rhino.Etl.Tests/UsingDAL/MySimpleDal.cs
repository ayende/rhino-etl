namespace Rhino.Etl.Tests.UsingDAL
{
    using System.Collections.Generic;

    public class MySimpleDal
    {
        public static List<User> Users = new List<User>();

        public static void Save(User user)
        {
            Users.Add(user);
        }

        public static IEnumerable<User> GetUsers()
        {
            yield return new User(1, "ayende", "ayende@example.org");
            yield return new User(2, "foo", "foo@example.org");
            yield return new User(3, "bar", "bar@example.org");
            yield return new User(4, "brak", "brak@example.org");
            yield return new User(5, "snar", "snar@example.org");
        }
    }
}