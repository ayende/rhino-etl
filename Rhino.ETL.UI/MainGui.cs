using System;
using System.IO;
using System.Windows.Forms;
using Rhino.ETL.UI.Commands;
using Rhino.ETL.UI.Panes;
using WeifenLuo.WinFormsUI;

namespace Rhino.ETL.UI
{
	using ICSharpCode.TextEditor.Document;

	public partial class MainGui : Form
	{
		public MainGui()
		{
			InitializeComponent();
			CommandDispatcher.Initialize(this);
			if(File.Exists("layout.xml"))
			{
				DockPanel.LoadFromXml("layout.xml",DeserializeContent);
			}
		}

		private static IDockContent DeserializeContent(string persistString)
		{
			Type type = Type.GetType(persistString);
			return (IDockContent)Activator.CreateInstance(type);
		}

		private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			DockPanel.SaveAsXml("layout.xml");
		}

		private void addPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new ItemPropertiesView().Show(DockPanel);
		}

		private void addDocsViewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new ProjectDocsTreeView().Show(DockPanel);
		}

		private void addLiveViewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new ProjectLiveView().Show(DockPanel);
		}

		public void EnsureLoggingGridVisible()
		{
			new LoggingPane().Show(DockPanel,DockState.DockBottom);
		}
	}
}