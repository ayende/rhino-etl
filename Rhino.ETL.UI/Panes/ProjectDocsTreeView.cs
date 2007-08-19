using System;
using System.Windows.Forms;
using Rhino.ETL.UI.Model;
using WeifenLuo.WinFormsUI;

namespace Rhino.ETL.UI.Panes
{
	public partial class ProjectDocsTreeView : DockContent
	{
		private readonly TreeNode root;

		public ProjectDocsTreeView()
		{
			InitializeComponent();
			root = DocsTree.Nodes.Find("Root", false)[0];
			EventHub.ProjectChanged += EventHub_ProjectChanged;
			EventHub_ProjectChanged(RetlProject.Instance);
		}

		void EventHub_ProjectChanged(RetlProject obj)
		{
			DocsTree.SuspendLayout();
			root.Nodes.Clear();
			root.Text = obj.Name;
			foreach (ProjectFolder folder in obj.Folders)
			{
				TreeNode folderNode = new TreeNode(folder.Name, 1, 1);
				root.Nodes.Add(folderNode);
				foreach (ProjectFile file in folder.Files)
				{
					TreeNode fileNode = new TreeNode(file.Name, 1, 1);
					folderNode.Nodes.Add(fileNode);
					
				}
			}
			root.Expand();
			DocsTree.ResumeLayout(true);
		}
	}
}