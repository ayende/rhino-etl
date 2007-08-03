using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using MbUnit.Framework;

namespace Rhino.ETL.Tests.Integration
{
	public class IntegrationTestBase : BaseTest
	{
		[SetUp]
		public virtual  void TestInitialize()
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
	}
}