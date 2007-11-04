namespace Rhino.ETL.Impl
{
	using System;
	using System.IO;
	using Boo.Lang.Compiler.Ast;
	using Rhino.Commons.Boo;

	public class AutoReferenceFilesAndAddToContextCompilerStep : AutoReferenceFilesCompilerStep
	{
		private readonly string finalName;

		public AutoReferenceFilesAndAddToContextCompilerStep(string baseDirectory, string finalName)
			: base(baseDirectory)
		{
			this.finalName = finalName;
		}

		public override void OnImport(Import node)
		{
			if (node.Namespace != "file")
				return;

			Module module = node.ParentNode as Module;

			base.OnImport(node);

			string alias = "_"+Guid.NewGuid().ToString("n");
			module.Imports.Add(new Import("", 
				new ReferenceExpression(Path.GetFileNameWithoutExtension(node.AssemblyReference.Name)), 
				new ReferenceExpression(alias)));

			MethodInvocationExpression newContext = new MethodInvocationExpression(AstUtil.CreateReferenceExpression(alias + "." + finalName));
			MethodInvocationExpression buildContext = new MethodInvocationExpression(
				new MemberReferenceExpression(new SelfLiteralExpression(), "ImportConfig"),
				newContext
				);
			module.Globals.Insert(0, buildContext);
		}
	}
}
