namespace Rhino.ETL2.Tests.UsingRetlang
{
	using Rhino.ETL.Engine;

	public class FakeJoin : Join
	{
		public FakeJoin(string name) : base(name)
		{
		}

		protected override void DoApply(Row Row, Row Left, Row Right)
		{
			Row["id"] = (int) Left["id"]*(int) Right["id"];
		}
	}
}