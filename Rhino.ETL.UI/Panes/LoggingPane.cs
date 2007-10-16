
namespace Rhino.ETL.UI.Panes
{
	using WeifenLuo.WinFormsUI;

	public class LoggingPane : DockContent
	{

		private Rhino.ETL.UI.Controls.LoggingGrid loggingGrid1;

		public LoggingPane()
		{
			InitializeComponent();
		}

		private void InitializeComponent()
		{
			this.loggingGrid1 = new Rhino.ETL.UI.Controls.LoggingGrid();
			this.SuspendLayout();
			// 
			// loggingGrid1
			// 
			this.loggingGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.loggingGrid1.Location = new System.Drawing.Point(0, 0);
			this.loggingGrid1.Name = "loggingGrid1";
			this.loggingGrid1.Size = new System.Drawing.Size(530, 307);
			this.loggingGrid1.TabIndex = 0;
			// 
			// LoggingPane
			// 
			this.ClientSize = new System.Drawing.Size(530, 307);
			this.Controls.Add(this.loggingGrid1);
			this.Name = "LoggingPane";
			this.TabText = "Logging";
			this.Text = "Logging";
			this.ResumeLayout(false);

		}
	}
}
