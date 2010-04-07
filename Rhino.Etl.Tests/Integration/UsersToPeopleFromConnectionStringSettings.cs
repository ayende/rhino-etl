using System.Configuration;

namespace Rhino.Etl.Tests.Integration
{
    using Core;

    public class UsersToPeopleFromConnectionStringSettings : EtlProcess
    {
        protected override void Initialize()
        {
            // Get the connection string settings for the test database
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings["test"];
            Register(new ReadUsers(connectionStringSettings));
            Register(new SplitName());
            Register(new WritePeople(connectionStringSettings));
        }
    }
}