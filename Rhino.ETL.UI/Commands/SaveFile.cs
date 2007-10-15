namespace Rhino.ETL.UI.Commands
{
	public class SaveFile : AbstractUICommand
	{
		public SaveFile(MainGui mainGui) : base(mainGui)
		{
		}

		public override void Execute()
		{
			Document document = (Document)Parent.DockPanel.ActiveDocument;
			document.SaveDocument();
		}
	}
}
