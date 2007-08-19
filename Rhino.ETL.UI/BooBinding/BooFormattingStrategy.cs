using System;
using System.Collections.Generic;
using System.Text;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Actions;
using ICSharpCode.TextEditor.Document;

namespace Rhino.ETL.UI.BooBinding
{
	public class BooFormattingStrategy : DefaultFormattingStrategy
	{
		// Methods
		public override void IndentLines(TextArea textArea, int begin, int end)
		{
		}

		protected override int SmartIndentLine(TextArea area, int line)
		{
			IDocument document = area.Document;
			LineSegment lineSegment = document.GetLineSegment(line - 1);
			if (document.GetText(lineSegment).EndsWith(":"))
			{
				LineSegment segment = document.GetLineSegment(line);
				string text = base.GetIndentation(area, line - 1) + Tab.GetIndentationString(document);
				document.Replace(segment.Offset, segment.Length, text + document.GetText(segment));
				return text.Length;
			}
			return base.SmartIndentLine(area, line);
		}
	}
}