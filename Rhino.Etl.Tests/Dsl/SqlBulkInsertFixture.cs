namespace Rhino.Etl.Tests.Dsl
{
	using System.Collections.Generic;
	using System.Data;
	using Commons;
	using Core;
	using MbUnit.Framework;

	[TestFixture]
	public class SqlBulkInsertFixture : BaseUserToPeopleTest
	{
		[Test]
		public void CanCompile()
		{
			using (EtlProcess process = CreateDslInstance("Dsl/UsersToPeopleBulk.boo"))
				Assert.IsNotNull(process);
		}

		[Test]
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