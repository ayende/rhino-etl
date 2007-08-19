using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Rhino.ETL.Engine;
using Rhino.ETL.UI.Model;
using WeifenLuo.WinFormsUI;

namespace Rhino.ETL.UI.Panes
{
	public partial class ProjectLiveView : DockContent
	{
		private readonly TreeNode sourcesNode;
		private readonly TreeNode connectionsNode;
		private readonly TreeNode targetsNode;
		private readonly TreeNode destinationsNode;
		private readonly TreeNode pipelinesNode;
		private readonly TreeNode joinsNode;
		private readonly TreeNode transformsNode;

		public ProjectLiveView()
		{
			InitializeComponent();
			sourcesNode = LiveTree.Nodes.Find("Sources", false)[0];
			connectionsNode = LiveTree.Nodes.Find("Connections", false)[0];
			targetsNode = LiveTree.Nodes.Find("Targets", false)[0];
			pipelinesNode = LiveTree.Nodes.Find("Pipelines", false)[0];
			destinationsNode = LiveTree.Nodes.Find("Destinations", false)[0];
			joinsNode = LiveTree.Nodes.Find("Joins", false)[0];
			transformsNode = LiveTree.Nodes.Find("Transforms", false)[0];

			foreach (TreeNode node in LiveTree.Nodes)
			{
				node.SelectedImageIndex = node.ImageIndex;
			}

			EventHub.ProjectChanged += RetlProject_Changed;
		}

		public void RetlProject_Changed(RetlProject project)
		{
			EtlConfigurationContext context = project.ConfigurationContext;
			ICollection<DataSource> sources = context.Sources.Values;
			ICollection<Connection> connections = context.Connections.Values;
			ICollection<Target> targets = context.Targets.Values;
			ICollection<Pipeline> pipelines = context.Pipelines.Values;
			ICollection<DataDestination> destinations = context.Destinations.Values;
			ICollection<Transform> transforms = context.Transforms.Values;
			ICollection<Join> joins = context.Joins.Values;

			LiveTree.SuspendLayout();
			ClearCurrentNodes();
			foreach (Transform value in transforms)
			{
				TreeNode node = new TreeNode(value.Name);
				node.Tag = value;
				node.ImageKey = connectionsNode.ImageKey;
				node.SelectedImageKey = connectionsNode.ImageKey;
				transformsNode.Nodes.Add(node);
			}
			foreach (Join value in joins)
			{
				TreeNode node = new TreeNode(value.Name);
				node.Tag = value;
				node.ImageKey = connectionsNode.ImageKey;
				node.SelectedImageKey = connectionsNode.ImageKey;
				joinsNode.Nodes.Add(node);
			}
			foreach (DataDestination value in destinations)
			{
				TreeNode node = new TreeNode(value.Name);
				node.Tag = value;
				node.ImageKey = connectionsNode.ImageKey;
				node.SelectedImageKey = connectionsNode.ImageKey;
				destinationsNode.Nodes.Add(node);
			}
			foreach (Pipeline value in pipelines)
			{
				TreeNode node = new TreeNode(value.Name);
				node.Tag = value;
				node.ImageKey = connectionsNode.ImageKey;
				node.SelectedImageKey = connectionsNode.ImageKey;
				pipelinesNode.Nodes.Add(node);
			}
			foreach (Target value in targets)
			{
				TreeNode node = new TreeNode(value.Name);
				node.Tag = value;
				node.ImageKey = connectionsNode.ImageKey;
				node.SelectedImageKey = connectionsNode.ImageKey;
				targetsNode.Nodes.Add(node);
			}
			foreach (Connection value in connections)
			{
				TreeNode node = new TreeNode(value.Name);
				node.Tag = value;
				node.ImageKey = connectionsNode.ImageKey;
				node.SelectedImageKey = connectionsNode.ImageKey;
				connectionsNode.Nodes.Add(node);
			}
			foreach (DataSource value in sources)
			{
				TreeNode node = new TreeNode(value.Name);
				node.Tag = value;
				node.ImageKey = sourcesNode.ImageKey;
				node.SelectedImageKey = sourcesNode.ImageKey;
				sourcesNode.Nodes.Add(node);
			}
			LiveTree.ResumeLayout(true);
		}

		private void ClearCurrentNodes()
		{
			sourcesNode.Nodes.Clear();
			connectionsNode.Nodes.Clear();
			targetsNode.Nodes.Clear();
			pipelinesNode.Nodes.Clear();
			destinationsNode.Nodes.Clear();
			joinsNode.Nodes.Clear();
			transformsNode.Nodes.Clear();
		}

		private void LiveTree_AfterSelect(object sender, TreeViewEventArgs e)
		{
			EventHub.RaiseItemChanged(e.Node.Tag);
		}
	}
}