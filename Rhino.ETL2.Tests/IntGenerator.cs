namespace Rhino.ETL2.Tests
{
	using Boo.Lang;
	using ETL;
	using Rhino.ETL.Engine;

	public class IntGenerator : ICallable
	{
		private readonly DataSource ds;

		public IntGenerator(DataSource ds)
		{
			this.ds = ds;
		}

		#region ICallable Members

		public object Call(object[] args)
		{
			for (int i = 0; i < 50; i++)
			{
				Row row = new Row();
				row["id"] = i;
				ds.SendRow(row);
			}
			return null;
		}

		#endregion
	}
}