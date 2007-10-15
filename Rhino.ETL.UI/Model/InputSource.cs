using System.IO;
using System.Text.RegularExpressions;
using Boo.Lang.Compiler.IO;

namespace Rhino.ETL.UI.Model
{
	public class InputSource
	{
		public Document Document;
		public FileInfo FileInfo;

		public InputSource(Document document, FileInfo fileInfo)
		{
			Document = document;
			FileInfo = fileInfo;
		}

		public Boo.Lang.Compiler.ICompilerInput CompilerInput
		{
			get
			{
				if(Document != null)
				{
					string name = Regex.Replace(Document.Name, @"#|$|\s", "");
					return new StringInput(name, Document.Code);
				}
				if(FileInfo!=null && FileInfo.Exists)
					return new FileInput(FileInfo.FullName);
				return null;
			}
		}
	}
}