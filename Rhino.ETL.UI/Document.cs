using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;

namespace Rhino.ETL.UI
{
	public partial class Document : Form
	{
		public Document()
		{
			InitializeComponent();
			TextEditor.Document.HighlightingStrategy = HighlightingManager.Manager.FindHighlighter("Rhino ETL");
		}
	}
}