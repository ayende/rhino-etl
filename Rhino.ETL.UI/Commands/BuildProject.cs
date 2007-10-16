using System;
using System.Collections.Generic;
using System.Text;
using Rhino.ETL.UI.Model;

namespace Rhino.ETL.UI.Commands
{
	public class BuildProject : AbstractUICommand
	{
		public BuildProject(MainGui mainGui) : base(mainGui)
		{
		}

		public override void Execute()
		{
			RetlProject.Instance.Build();
		}
	}
}
