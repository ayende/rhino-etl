using System.Windows.Forms;

namespace Rhino.ETL.UI.Commands
{
	public class CreateNewFile : AbstractUICommand
	{
		public CreateNewFile(MainGui parent) : base(parent)
		{
		}

		public override void Execute()
		{
			Document d = new Document();
			d.MdiParent = Parent;
			d.Show();
		}
	}
}