namespace Rhino.Etl.Tests.UsingDAL
{
    using System.Collections.Generic;
    using System.IO;
    using Core;
    using MbUnit.Framework;

    [TestFixture]
    public class UsingDALFixture
    {
        private const string expected =
            @"Id	Name	Email
1	ayende	ayende@example.org
2	foo	foo@example.org
3	bar	bar@example.org
4	brak	brak@example.org
5	snar	snar@example.org
";
        [Test]
        public void CanWriteToFileFromDAL()
        {
            ExportUsersToFile export = new ExportUsersToFile();
            export.Execute();
            string actual = File.ReadAllText("users.txt");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void CanReadFromFileToDAL()
        {
            MySimpleDal.Users = new List<User>();
            File.WriteAllText("users.txt", expected);

            ImportUsersFromFile import = new ImportUsersFromFile();
            import.Execute();

            Assert.AreEqual(5, MySimpleDal.Users.Count);
        }
    }
}