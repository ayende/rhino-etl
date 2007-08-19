using System.Windows.Forms;

namespace Rhino.ETL.UI.Commands
{
	public abstract class AbstractUICommand
	{
		private readonly MainGui parent;

		public MainGui Parent
		{
			get { return parent; }
		}

		public abstract void Execute();

		protected AbstractUICommand(MainGui mainGui)
		{
			this.parent = mainGui;
		}
	}
}