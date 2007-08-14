using System.Windows.Forms;

namespace Rhino.ETL.UI.Commands
{
	public abstract class AbstractUICommand
	{
		private readonly Form parent;

		public Form Parent
		{
			get { return parent; }
		}

		public abstract void Execute();

		protected AbstractUICommand(Form mainGui)
		{
			this.parent = mainGui;
		}
	}
}