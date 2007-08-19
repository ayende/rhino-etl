using System;
using System.IO;
using ICSharpCode.TextEditor.Document;
using Rhino.ETL.UI.BooBinding;
using Rhino.ETL.UI.Model;
using WeifenLuo.WinFormsUI;

namespace Rhino.ETL.UI
{
	public partial class Document : DockContent
	{
		private static int Counter = 1;
		public string DocumentName = "Document #" + Counter++;
		private bool isChanged = false;
		private InputSource inputSource;

		public InputSource InputSource
		{
			get { return inputSource; }
			set { inputSource = value; }
		}

		public Document()
		{
			InitializeComponent();
			TextEditor.TextChanged += Changed;
			TextEditor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("Rhino ETL");
			TextEditor.Document.FormattingStrategy = new BooFormattingStrategy();
		}

		private void Changed(object sender, EventArgs e)
		{
			isChanged = true;
		}

		public string Code
		{
			get { return TextEditor.Text; }
		}

		public bool IsChanged
		{
			get { return isChanged; }
		}

		public void LoadFile(string name)
		{
			TextEditor.LoadFile(name, true, true);
			inputSource.Document = this;
			Changed(this, EventArgs.Empty);
		}
	}
}