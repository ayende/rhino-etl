namespace Rhino.ETL2.Tests.UsingRetlang
{
	using Rhino.ETL.Engine;
	using Rhino.ETL.Impl;

	public class FakeTransform : Transform
	{
		public FakeTransform(string name) : base(name)
		{
		}

		protected override void DoApply(Row Row, QuackingDictionary Parameters)
		{
			Row["id"] = string.Format("Id: {0}", Row["Id"]);
		}
	}
}