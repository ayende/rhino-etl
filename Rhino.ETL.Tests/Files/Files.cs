using System;
using System.Collections;
using MbUnit.Framework;
using Rhino.ETL.Tests.Joins;

namespace Rhino.ETL.Tests.Files
{
	[TestFixture]
	public class Files : BaseTest
	{
		private EtlConfigurationContext configurationContext;

		[Test]
		public void ReadingAndJoiningFromFile()
		{
			configurationContext = BuildContext(@"Files\FromFile.retl");
			ExecutionPackage executionPackage = configurationContext.BuildPackage();
			ExecutionResult executionResult = executionPackage.Execute("default");
			Assert.AreEqual(executionResult.Status, ExecutionStatus.Success);
		}
	}
}