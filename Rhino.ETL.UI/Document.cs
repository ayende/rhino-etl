using System;
using System.IO;
using ICSharpCode.TextEditor.Document;
using Rhino.ETL.UI.BooBinding;
using Rhino.ETL.UI.Model;
using WeifenLuo.WinFormsUI;

namespace Rhino.ETL.UI
{
	using System.Windows.Forms;

	public partial class Document : DockContent
	{
		private static int Counter = 1;
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
			Text = Name = "Document #" + Counter++; ;
			TextEditor.ActiveTextAreaControl.Document.DocumentChanged += Changed;
			TextEditor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("Rhino ETL");
			TextEditor.Document.FormattingStrategy = new BooFormattingStrategy();
		}

		private void Changed(object sender, EventArgs e)
		{
			isChanged = true;
			if (Text.EndsWith(" *") == false)
				Text = Text + " *";
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			if (IsChanged == false)
				return;
			DialogResult result = MessageBox.Show(string.Format("Save changes to {0}?", Name), "Unsaved cahnges", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
			if (result == DialogResult.Cancel)
				e.Cancel = true;
			if (result == DialogResult.Yes)
				SaveDocument();
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
			ResetChange();
		}

		public void SaveDocument()
		{
			if (string.IsNullOrEmpty(TextEditor.FileName))
			{
				if (GetFileNameFromUser() == false)
					return;
			}
			TextEditor.SaveFile(TextEditor.FileName);
			ResetChange();
		}

		private void ResetChange()
		{
			SetDocumentName();
			isChanged = false;
		}

		private void SetDocumentName()
		{
			Name = Text = Path.GetFileNameWithoutExtension(TextEditor.FileName);
		}

		private bool GetFileNameFromUser()
		{
			using (SaveFileDialog sfd = new SaveFileDialog())
			{
				sfd.Filter = "Rhino ETL | *.retl";
				if (sfd.ShowDialog(this) == DialogResult.Cancel)
					return false;
				TextEditor.FileName = sfd.FileName;
				return true;
			}
		}
	}
}