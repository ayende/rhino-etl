using System.Windows.Forms;
using Rhino.ETL.UI.Commands;

namespace Rhino.ETL.UI.Controls
{
	public class CommandAwareToolStripButton : ToolStripButton
	{
		private string command;

		public CommandAwareToolStripButton()
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