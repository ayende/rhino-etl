namespace Rhino.ETL.UI.Controls
{
	partial class LoggingGrid
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.grid = new System.Windows.Forms.ListView();
			this.Time = new System.Windows.Forms.ColumnHeader();
			this.Message = new System.Windows.Forms.ColumnHeader();
			this.Level = new System.Windows.Forms.ColumnHeader();
			this.Exception = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// grid
			// 
			this.grid.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Time,
            this.Message,
            this.Level,
            this.Exception});
			this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.grid.FullRowSelect = true;
			this.grid.GridLines = true;
			this.grid.Location = new System.Drawing.Point(0, 0);
			this.grid.MultiSelect = false;
			this.grid.Name = "grid";
			this.grid.Size = new System.Drawing.Size(476, 277);
			this.grid.TabIndex = 0;
			this.grid.UseCompatibleStateImageBehavior = false;
			this.grid.View = System.Windows.Forms.View.Details;
			// 
			// Time
			// 
			this.Time.Text = "Time";
			// 
			// Message
			// 
			this.Message.Text = "Message";
			this.Message.Width = 55;
			// 
			// Level
			// 
			this.Level.Text = "Level";
			// 
			// Exception
			// 
			this.Exception.Text = "Exception";
			// 
			// LoggingGrid
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.grid);
			this.Name = "LoggingGrid";
			this.Size = new System.Drawing.Size(476, 277);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView grid;
		private System.Windows.Forms.ColumnHeader Time;
		private System.Windows.Forms.ColumnHeader Message;
		private System.Windows.Forms.ColumnHeader Level;
		private System.Windows.Forms.ColumnHeader Exception;
	}
}
