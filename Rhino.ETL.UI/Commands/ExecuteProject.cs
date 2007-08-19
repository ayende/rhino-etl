using System;
using System.Collections.Generic;
using System.Text;
using Rhino.ETL.UI.Model;

namespace Rhino.ETL.UI.Commands
{
	public class ExecuteProject : AbstractUICommand
	{
		public ExecuteProject(MainGui mainGui) : base(mainGui)
		{
		}

		public override void Execute()
		{
			RetlProject.Instance.Execute();
		}
	}
}
