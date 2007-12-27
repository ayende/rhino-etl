namespace Rhino.ETL.Impl
{
	using System.IO;
	using Boo.Lang.Compiler.Ast;
	using DSL;

	public class AutoReferenceFilesAndAddToContextCompilerStep : AutoReferenceFilesCompilerStep
	{
		public override void OnImport(Import node)
		{
			if (node.Namespace != "file")
				return;

			Module module = (Module)node.ParentNode;

			base.OnImport(node);

			string alias = Path.GetFileNameWithoutExtension(node.AssemblyReference.Name);

			MethodInvocationExpression newContext = new MethodInvocationExpression(AstUtil.CreateReferenceExpression(alias));
			MethodInvocationExpression buildContext = new MethodInvocationExpression(
				new MemberReferenceExpression(new SelfLiteralExpression(), "ImportConfig"),
				newContext
				);
			module.Globals.Insert(0, buildContext);
		}
	}
}
