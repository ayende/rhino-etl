namespace Rhino.Etl.Tests.Branches
{
	using System.Data;
	using Commons;
	using MbUnit.Framework;

	[TestFixture]
	public class BranchesFixture : BaseFibonacciTest
	{
		[Test]
		public void CanBranchThePipeline()
		{
			using (BranchingProcess process = new BranchingProcess())
				process.Execute();

			AssertCountForFibonacci();
		}

		protected static void AssertCountForFibonacci()
		{
			int max = Use.Transaction<int>("test", delegate(IDbCommand cmd)
			{
				cmd.CommandText = "SELECT count(*) FROM Fibonacci";
				return (int)cmd.ExecuteScalar();
			});
			Assert.AreEqual(60, max);
		}
	}
}