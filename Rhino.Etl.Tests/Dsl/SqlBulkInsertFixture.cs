using Rhino.Etl.Core.Infrastructure;

namespace Rhino.Etl.Tests.Dsl
{
	using System.Collections.Generic;
	using System.Data;
	using Core;
	using Xunit;

	
	public class SqlBulkInsertFixture : BaseUserToPeopleTest
	{
		[Fact]
		public void CanCompile()
		{
			using (EtlProcess process = CreateDslInstance("Dsl/UsersToPeopleBulk.boo"))
				Assert.NotNull(process);
		}

		[Fact]
		public void CanCopyTableWithTransform()
		{
			using (EtlProcess process = CreateDslInstance("Dsl/UsersToPeopleBulk.boo"))
				process.Execute();

			List<string[]> names = Use.Transaction<List<string[]>>("test", delegate(IDbCommand cmd)
			{
				List<string[]> tuples = new List<string[]>();
				cmd.CommandText = "SELECT firstname, lastname from people order by userid";
				using (IDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						tuples.Add(new string[] { reader.GetString(0), reader.GetString(1) });
					}
				}
				return tuples;
			});
			AssertNames(names);
		}
	}
}