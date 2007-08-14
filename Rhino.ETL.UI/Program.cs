using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ICSharpCode.TextEditor.Document;

namespace Rhino.ETL.UI
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			HighlightingManager.Manager.AddSyntaxModeFileProvider(new FileSyntaxModeProvider(
				AppDomain.CurrentDomain.BaseDirectory));

			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainGui());
		}
	}
}