using System;
using System.Configuration;
using System.IO;
using MbUnit.Framework;
using System.Data.SqlClient;
using System.Data;

namespace Rhino.ETL.Tests.Integration
{
	[TestFixture]
	public class IntegrationTests : BaseTest
	{

		[SetUp]
		public void TestInitialize()
		{
			ExecuteCommand(delegate(IDbCommand com)
			{
				com.CommandText = File.ReadAllText(
					Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
					@"Integration\Database.sql")
					);
				com.ExecuteNonQuery();
			});

		}

		[Test]
		public void WillCopyRowsFromOneTableToAnother()
		{
			ExecutePackage("basic");
			AssertRowNotZero();
		}

		[Test]
		public void WillSkipSingleRowWithBadEmail()
		{
			ExecutePackage("basic");
			AssertRowCount(8);
		}

		public void AssertRowNotZero()
		{
			ExecuteCommand(delegate(IDbCommand com)
			{
				com.CommandText = "SELECT COUNT(*) FROM Users_Destination";
				int count = Convert.ToInt32(com.ExecuteScalar());
				Assert.AreNotEqual(0, count);
			});
		}

		public void AssertRowCount(int expectedCount)
		{
			ExecuteCommand(delegate(IDbCommand com)
			{
				com.CommandText = "SELECT COUNT(*) FROM Users_Destination";
				int count = Convert.ToInt32(com.ExecuteScalar());
				Assert.AreEqual(expectedCount, count);
			});
		}

		private static void ExecuteCommand(Action<IDbCommand> action)
		{
			string connectionString = ConfigurationManager.ConnectionStrings["TestDatabase"].ConnectionString;
			using (SqlConnection con = new SqlConnection(connectionString))
			{
				con.Open();
				using (IDbCommand com = con.CreateCommand())
				{
					action(com);
				}
			}
		}

		private static void ExecutePackage(string name)
		{
			EtlConfigurationContext configurationContext = BuildContext(@"Integration\" + name + ".retl");
			ExecutionPackage package = configurationContext.BuildPackage();
			package.Execute();
		}
	}
}
