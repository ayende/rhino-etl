using System.Windows.Forms;
using Rhino.ETL.UI.Model;

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
			d.InputSource = RetlProject.Instance.AddDocument(d);
			d.Show(Parent.DockPanel);
		}
	}
}