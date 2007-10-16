namespace Rhino.ETL.UI.Commands
{
	using Model;

	public class ExecuteProject : AbstractUICommand
	{
		public ExecuteProject(MainGui mainGui) : base(mainGui)
		{
		}

		public override void Execute()
		{
			Parent.EnsureLoggingGridVisible();
			RetlProject.Instance.Build();
			RetlProject.Instance.Execute();
		}
	}
}
