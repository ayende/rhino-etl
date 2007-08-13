using System;
using System.Collections;
using System.IO;
using System.Text;
using MbUnit.Core.Exceptions;
using MbUnit.Framework;
using Rhino.ETL.Tests.Integration;
using Rhino.ETL.Tests.Joins;

namespace Rhino.ETL.Tests.Files
{
	[TestFixture]
	public class Files : IntegrationTestBase
	{
		private EtlConfigurationContext configurationContext;

		[Test]
		public void ReadingAndJoiningFromFile()
		{
			configurationContext = BuildContext(@"Files\FromFile.retl");
			ExecutionPackage executionPackage = configurationContext.BuildPackage();
			ExecutionResult executionResult = executionPackage.Execute("default");
			AssertSuccess(executionResult);

			AssertRowCount("OrdersWareHousing", 3);
		}

		[Test]
		public void ReadingFromDatabaseAndWritingToFile()
		{
			configurationContext = BuildContext(@"Files\ToFile.retl");
			ExecutionPackage executionPackage = configurationContext.BuildPackage();
			ExecutionResult executionResult = executionPackage.Execute("default");
			AssertSuccess(executionResult);

			string actual = File.ReadAllText("output.txt");
			string expected = @"10248	VINET	5
10249	TOMSP	6
10250	HANAR	4
";
			Assert.AreEqual(expected, actual );
		}

		private void AssertSuccess(ExecutionResult executionResult)
		{
			if(executionResult.Status!=ExecutionStatus.Success)
			{
				StringBuilder sb = new StringBuilder();
				foreach (Exception exception in executionResult.Exceptions)
				{
					sb.AppendLine(exception.Message);
				}
				throw new AssertionException(sb.ToString());
			}
		}
	}
}