namespace Rhino.Etl.Tests.UsingDAL
{
    using FileHelpers;

    [DelimitedRecord("\t"), IgnoreFirst]
    public class UserRecord
    {
        public int Id;
        public string Name;
        public string Email;
    }
}