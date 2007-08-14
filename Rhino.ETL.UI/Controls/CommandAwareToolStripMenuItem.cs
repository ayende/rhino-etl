using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Rhino.ETL.UI.Commands;

namespace Rhino.ETL.UI.Controls
{
	public class CommandAwareToolStripMenuItem : ToolStripMenuItem
	{
		private string command;

		public CommandAwareToolStripMenuItem()
		{
			this.Click += delegate
			              	{
								CommandDispatcher.Dispatch(Command);
			              	};
		}

		public string Command
		{
			get { return command; }
			set { command = value; }
		}
	}
}
