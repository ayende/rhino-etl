namespace Rhino.ETL2.Tests
{
	using Rhino.ETL.Engine;

	public class FakeEtlConfigurationContext : EtlConfigurationContext
	{
		private readonly string name = "fake";

		public override string Name
		{
			get { return name; }
		}

		public override void BuildConfig()
		{
		}
	}
}