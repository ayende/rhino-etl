namespace Rhino.Etl.Tests.LoadTest
{
	using System;
	using System.Collections.Generic;
	using Core;
	using MbUnit.Framework;
	using Rhino.Etl.Core.Operations;

	[TestFixture]
	public class LoadTestJoinsFixture
	{
		[Test]
		public void CanDoLargeJoinsefficently()
		{
			DateTime start = DateTime.Now;
			using(Join_250_000_UsersWithMostlyFallingOut proc = new Join_250_000_UsersWithMostlyFallingOut())
			{
				proc.Execute();
				Assert.AreEqual(15000, proc.operation.count);
			}
			TimeSpan span = DateTime.Now-start;
			Assert.Less(span.TotalSeconds, 1);
		}
	}
}
    