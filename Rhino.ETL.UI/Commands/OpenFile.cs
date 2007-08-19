using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Rhino.ETL.UI.Model;

namespace Rhino.ETL.UI.Commands
{
	public class OpenFile : AbstractUICommand
	{
		public OpenFile(MainGui mainGui)
			: base(mainGui)
		{
		}


		public override void Execute()
		{
			using (OpenFileDialog ofd = new OpenFileDialog())
			{
				ofd.Filter = "Rhino ETL|*.retl";
				ofd.Multiselect = false;
				if (ofd.ShowDialog(Parent) == DialogResult.Cancel)
					return;

				Document d = new Document();
				d.InputSource = RetlProject.Instance.GetSourceFor(ofd.FileName);
				d.LoadFile(ofd.FileName);
				d.Show(Parent.DockPanel);
			}
		}
	}
}
