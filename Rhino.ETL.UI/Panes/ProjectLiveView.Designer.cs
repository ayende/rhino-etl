namespace Rhino.ETL.UI.Panes
{
	partial class ProjectLiveView
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Connections");
			System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Sources");
			System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Destinations");
			System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Joins");
			System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Transforms");
			System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Pipelines");
			System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Targets");
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectLiveView));
			this.LiveTree = new System.Windows.Forms.TreeView();
			this.ProjectIcons = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// LiveTree
			// 
			this.LiveTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.LiveTree.ImageIndex = 0;
			this.LiveTree.ImageList = this.ProjectIcons;
			this.LiveTree.Location = new System.Drawing.Point(0, 0);
			this.LiveTree.Name = "LiveTree";
			treeNode1.ImageIndex = 8;
			treeNode1.Name = "Connections";
			treeNode1.Text = "Connections";
			treeNode2.ImageIndex = 1;
			treeNode2.Name = "Sources";
			treeNode2.Text = "Sources";
			treeNode3.ImageIndex = 14;
			treeNode3.Name = "Destinations";
			treeNode3.Text = "Destinations";
			treeNode4.ImageIndex = 13;
			treeNode4.Name = "Joins";
			treeNode4.Text = "Joins";
			treeNode5.ImageIndex = 10;
			treeNode5.Name = "Transforms";
			treeNode5.Text = "Transforms";
			treeNode6.ImageIndex = 12;
			treeNode6.Name = "Pipelines";
			treeNode6.Text = "Pipelines";
			treeNode7.ImageIndex = 9;
			treeNode7.Name = "Targets";
			treeNode7.Text = "Targets";
			this.LiveTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode5,
            treeNode6,
            treeNode7});
			this.LiveTree.SelectedImageIndex = 0;
			this.LiveTree.Size = new System.Drawing.Size(211, 465);
			this.LiveTree.TabIndex = 0;
			this.LiveTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.LiveTree_AfterSelect);
			// 
			// ProjectIcons
			// 
			this.ProjectIcons.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ProjectIcons.ImageStream")));
			this.ProjectIcons.TransparentColor = System.Drawing.Color.Magenta;
			this.ProjectIcons.Images.SetKeyName(0, "database_connect.png");
			this.ProjectIcons.Images.SetKeyName(1, "database_table.png");
			this.ProjectIcons.Images.SetKeyName(2, "database.png");
			this.ProjectIcons.Images.SetKeyName(3, "database_add.png");
			this.ProjectIcons.Images.SetKeyName(4, "database_connect.png");
			this.ProjectIcons.Images.SetKeyName(5, "database_delete.png");
			this.ProjectIcons.Images.SetKeyName(6, "database_edit.png");
			this.ProjectIcons.Images.SetKeyName(7, "database_error.png");
			this.ProjectIcons.Images.SetKeyName(8, "database_gear.png");
			this.ProjectIcons.Images.SetKeyName(9, "database_go.png");
			this.ProjectIcons.Images.SetKeyName(10, "database_key.png");
			this.ProjectIcons.Images.SetKeyName(11, "database_lightning.png");
			this.ProjectIcons.Images.SetKeyName(12, "database_link.png");
			this.ProjectIcons.Images.SetKeyName(13, "database_refresh.png");
			this.ProjectIcons.Images.SetKeyName(14, "database_save.png");
			// 
			// ProjectLiveView
			// 
			this.ClientSize = new System.Drawing.Size(211, 465);
			this.Controls.Add(this.LiveTree);
			this.Name = "ProjectLiveView";
			this.TabText = "Live View";
			this.Text = "Live View";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView LiveTree;
		private System.Windows.Forms.ImageList ProjectIcons;
	}
}