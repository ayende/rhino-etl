using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Rhino.ETL.UI.Commands
{
	public class OpenFile : AbstractUICommand
	{
		public OpenFile(Form mainGui) : base(mainGui)
		{
		}


		public override void Execute()
		{
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Filter = "Rhino ETL|*.retl";
			ofd.Multiselect = false;
			if(ofd.ShowDialog(Parent)==DialogResult.Cancel)
				return;

			Document d = new Document();
			d.MdiParent = Parent;
			d.Text = Path.GetFileName(ofd.FileName);
			d.TextEditor.LoadFile(ofd.FileName, true, true);
			d.Show();
		}
	}
}
 