namespace Rhino.ETL.UI.Panes
{
	partial class ProjectDocsTreeView
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
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Project");
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectDocsTreeView));
			this.DocsTree = new System.Windows.Forms.TreeView();
			this.ImageList = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// DocsTree
			// 
			this.DocsTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.DocsTree.ImageIndex = 0;
			this.DocsTree.ImageList = this.ImageList;
			this.DocsTree.Location = new System.Drawing.Point(0, 0);
			this.DocsTree.Name = "DocsTree";
			treeNode1.ImageKey = "report_user.png";
			treeNode1.Name = "Root";
			treeNode1.SelectedImageKey = "report_user.png";
			treeNode1.Text = "Project";
			this.DocsTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
			this.DocsTree.SelectedImageIndex = 0;
			this.DocsTree.Size = new System.Drawing.Size(159, 422);
			this.DocsTree.TabIndex = 0;
			// 
			// ImageList
			// 
			this.ImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ImageList.ImageStream")));
			this.ImageList.TransparentColor = System.Drawing.Color.Transparent;
			this.ImageList.Images.SetKeyName(0, "report_user.png");
			this.ImageList.Images.SetKeyName(1, "folder.png");
			this.ImageList.Images.SetKeyName(2, "page_code.png");
			// 
			// ProjectDocsTreeView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(159, 422);
			this.Controls.Add(this.DocsTree);
			this.Name = "ProjectDocsTreeView";
			this.TabText = "ProjectDocsTreeView";
			this.Text = "ProjectDocsTreeView";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView DocsTree;
		private System.Windows.Forms.ImageList ImageList;
	}
}