namespace Rhino.Etl.Tests.Infrastructure
{
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using Rhino.Etl.Core.Infrastructure;
    using Xunit;

    public class UseFixture
    {
        [Fact]
        public void SupportsAssemblyQualifiedConnectionTypeNameAsProviderNameInConnectionStringSettings()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["test"].ConnectionString;
            ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings("test2", connectionString, typeof(SqlConnection).AssemblyQualifiedName);

            using (IDbConnection connection = Use.Connection(connectionStringSettings))
            {
                Assert.NotNull(connection);
            }
        }
 
        [Fact]
        public void SupportsProviderNameInConnectionStringSettings()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["test"].ConnectionString;
            ConnectionStringSettings connectionStringSettings = new ConnectionStringSettings("test2", connectionString, "System.Data.SqlClient");

            using (IDbConnection connection = Use.Connection(connectionStringSettings))
            {
                Assert.NotNull(connection);
            }
        }
    }
}
