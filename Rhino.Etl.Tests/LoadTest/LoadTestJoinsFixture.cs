namespace Rhino.Etl.Tests.LoadTest
{
	using System.Diagnostics;
	using MbUnit.Framework;

	[TestFixture]
	public class LoadTestJoinsFixture
	{
		[Test]
        [Ignore("It depends too much of what the machine is doing and how powerful it is")]
		public void CanDoLargeJoinsefficently()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			using(Join_250_000_UsersWithMostlyFallingOut proc = new Join_250_000_UsersWithMostlyFallingOut())
			{
				proc.Execute();
				Assert.AreEqual(15000, proc.operation.count);
			}
			stopwatch.Stop();
			Assert.Less(stopwatch.ElapsedMilliseconds, 1000);
		}
	}
}
    