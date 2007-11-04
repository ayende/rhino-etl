using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;

namespace Rhino.ETL.Impl
{
	public class CommandGeneratorMacro : AbstractAstMacro
	{
		public static readonly object Key = new object();

		public override Statement Expand(MacroStatement macro)
		{
			MacroStatement parentSource = macro.ParentNode.ParentNode as MacroStatement;
			if (parentSource == null ||
				parentSource.Name.Equals("source", StringComparison.InvariantCultureIgnoreCase) == false)
			{
				Errors.Add(
					new CompilerError(macro.LexicalInfo, "A command generator statement can appear only under a source statement", null));
				return null;
			}
			BlockExpression blockExpression = new BlockExpression();
			if (macro.Block.Statements.Count == 1 && macro.Block.Statements[0] is ExpressionStatement)
			{
				ExpressionStatement statement = (ExpressionStatement)macro.Block.Statements[0];
				blockExpression.Body.Add(new ReturnStatement(statement.Expression));
			}
			else
			{
				blockExpression.Body = macro.Block;
			}
			blockExpression.ReturnType = CodeBuilder.CreateTypeReference(typeof(string));
			parentSource[Key] = blockExpression;
			return null;
		}
	}
}