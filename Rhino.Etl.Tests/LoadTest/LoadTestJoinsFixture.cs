namespace Rhino.Etl.Tests.LoadTest
{
	using System.Diagnostics;
	using Xunit;

	
	public class LoadTestJoinsFixture
	{
        [Fact(Skip = "It depends too much of what the machine is doing and how powerful it is")]
        public void CanDoLargeJoinsefficently()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			using(Join_250_000_UsersWithMostlyFallingOut proc = new Join_250_000_UsersWithMostlyFallingOut())
			{
				proc.Execute();
				Assert.Equal(15000, proc.operation.count);
			}
			stopwatch.Stop();
			Assert.True(stopwatch.ElapsedMilliseconds < 1000);
		}
	}
}
    