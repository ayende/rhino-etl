namespace Rhino.ETL.UI
{
	partial class Document
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
			this.TextEditor = new ICSharpCode.TextEditor.TextEditorControl();
			this.SuspendLayout();
			// 
			// TextEditor
			// 
			this.TextEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.TextEditor.Location = new System.Drawing.Point(0, 0);
			this.TextEditor.Name = "TextEditor";
			this.TextEditor.ShowEOLMarkers = true;
			this.TextEditor.ShowInvalidLines = false;
			this.TextEditor.ShowSpaces = true;
			this.TextEditor.ShowTabs = true;
			this.TextEditor.ShowVRuler = true;
			this.TextEditor.Size = new System.Drawing.Size(752, 495);
			this.TextEditor.TabIndex = 1;
			// 
			// Document
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(752, 495);
			this.Controls.Add(this.TextEditor);
			this.Name = "Document";
			this.Text = "Document";
			this.ResumeLayout(false);

		}

		#endregion

		public ICSharpCode.TextEditor.TextEditorControl TextEditor;

	}
}