namespace Rhino.Etl.Tests.UsingDAL
{
    public class User
    {
        private string email;
        private int id;
        private string name;

        public User()
        {

        }

        public User(int id, string name, string email)
        {
            this.id = id;
            this.name = name;
            this.email = email;
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}