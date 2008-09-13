namespace Rhino.Etl.Tests.LoadTest
{
	using System.Diagnostics;
	using MbUnit.Framework;

	[TestFixture]
	public class LoadTestJoinsFixture
	{
		[Test]
		public void CanDoLargeJoinsefficently()
		{
			var stopwatch = Stopwatch.StartNew();
			using(var proc = new Join_250_000_UsersWithMostlyFallingOut())
			{
				proc.Execute();
				Assert.AreEqual(15000, proc.operation.count);
			}
			stopwatch.Stop();
			Assert.Less(stopwatch.ElapsedMilliseconds, 1000);
		}
	}
}
    