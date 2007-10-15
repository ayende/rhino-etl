using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Data.SqlClient;
using MbUnit.Framework;

namespace Rhino.ETL.Tests
{
	using Engine;

	[TestFixture]
	public class ConnectionFixture : BaseTest
	{
		private EtlConfigurationContext configurationContext;

		[SetUp]
		public void TestInitialize()
		{
			configurationContext = BuildContext(@"Connections\connection_only.retl");
			configurationContext.EnterContext();
		}

		[TearDown]
		public void TestCleanup()
		{
			EtlConfigurationContext.Current = null;
		}

		[Test]
		public void EvaluatingScript_WithConnection_WillAddConnectionToContext()
		{
			Assert.AreEqual(3, configurationContext.Connections.Count, "should have three connections");
		}

		[Test]
		public void EtlContext_IsNamed_AccordingToFileName()
		{
			Assert.AreEqual("connection_only", configurationContext.Name);
		}

		[Test]
		public void Connections_FromConfiguration_AreNamed()
		{
			Assert.IsTrue(configurationContext.Connections.ContainsKey("Northwind"));
			Assert.IsTrue(configurationContext.Connections.ContainsKey("SouthSand"));
		}

		[Test]
		public void Connections_Names_AreCaseInsensitive()
		{
			Assert.IsTrue(configurationContext.Connections.ContainsKey("NORTHWIND"));
			Assert.IsTrue(configurationContext.Connections.ContainsKey("southsand"));
		}

		[Test]
		public void Connections_NamesAndKeys_AreIdentical()
		{
			foreach (KeyValuePair<string, Connection> pair in configurationContext.Connections)
			{
				Assert.AreEqual(pair.Key, pair.Value.Name);
			}
		}

		[Test]
		public void Connections_ConnectionString_ConfiguredFromDSL()
		{
			Assert.AreEqual("Data Source=localhost;Initial Catalog=ETL_Test; Integrated Security=SSPI;",
				configurationContext.Connections["Northwind"].ConnectionString);
		}

		[Test]
		public void Connections_ConnectionStringName_ConfiguredFromDSL()
		{
			Assert.AreEqual("SouthSand",
				configurationContext.Connections["SouthSand"].ConnectionStringName);
		}

		[Test]
		public void Connections_WhenConnectionStringNameSpecified_ReturnConfigValueFromConfiguartion()
		{
			Assert.AreEqual("Data Source=hr;User ID=scott;Password=tiger;",
				configurationContext.Connections["SouthSand"].ConnectionString);
		}

		[Test]
		[ExpectedException(typeof(ConfigurationErrorsException), "[Connection: SouthSand] Named connection string 'invalid' does not exists")]
		public void Connection_WhenInvalidConnectionStringSpecified_WillThrowOnAccessToConnectionString()
		{
			configurationContext.Connections["SouthSand"].ConnectionStringName = "invalid";
			string fake = configurationContext.Connections["SouthSand"].ConnectionString;
			GC.KeepAlive(fake);//fake, will never get here, but prevents unused variable errors;
		}

		[Test]
		public void Connections_ConcurrentConnections_ConfiguredFromDSL()
		{
			Assert.AreEqual(5, configurationContext.Connections["Northwind"].ConcurrentConnections);
		}

		[Test]
		public void Connections_ConccurrentConnectionsWhenNotSpecified_GreaterThanZero()
		{
			Assert.Greater(
				configurationContext.Connections["southsand"].ConcurrentConnections,
				0);
		}

		[Test]
		public void Connections_ConnectionStringGenerator_CanUseEvnrionmentVariables()
		{
			Environment.SetEnvironmentVariable("MyEnvVar", "MyExpectedValue");

			Assert.AreEqual(
					"MyExpectedValue",
					configurationContext.Connections["StrangeOne"].ConnectionString
			);

			Environment.SetEnvironmentVariable("MyEnvVar", "2");

			Assert.AreEqual(
					"2",
					configurationContext.Connections["StrangeOne"].ConnectionString
			);
		}

		[Test]
		public void Connections_ConnectionType_CanBeConfiguredConnectionViaDSL()
		{
			Assert.AreEqual(
				typeof(OracleConnection),
				configurationContext.Connections["southsand"].ConnectionType
				);
		}

		[Test]
		public void GettingDbConnectionWillReturnOpenConnection()
		{
			Connection con = new Connection("test");
            con.ConnectionStringName = "TestDatabase";
			con.ConnectionType = typeof (SqlConnection);
			IDbConnection connection = con.TryAcquire();
			Assert.AreEqual(ConnectionState.Open, connection.State);
			connection.Dispose();
		}

		[Test]
		public void ReleaseConnectionWillCloseTheDbConnection()
		{
			Connection con = new Connection("test");
            con.ConnectionStringName = "TestDatabase";
			con.ConnectionType = typeof(SqlConnection); 
			IDbConnection connection = con.TryAcquire();
			con.Release(connection);
			Assert.AreEqual(ConnectionState.Closed, connection.State);
		}

		[Test]
		public void CannotAcquireMoreThanConcurrentConnectionsLimit()
		{
			Connection con = new Connection("test");
            con.ConnectionStringName = "TestDatabase";
			con.ConcurrentConnections = 1;
			con.ConnectionType = typeof(SqlConnection);
			IDbConnection connection = con.TryAcquire();
			Assert.IsNull(con.TryAcquire());
			con.Release(connection);
			connection = con.TryAcquire();
			Assert.IsNotNull(connection);
			con.Release(connection);
		}
	}
}