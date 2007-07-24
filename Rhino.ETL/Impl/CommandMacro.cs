using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;

namespace Rhino.ETL.Impl
{
	public class CommandMacro : AbstractAstMacro
	{
		public static readonly object Key = new object();

		public override Statement Expand(MacroStatement macro)
		{
			MacroStatement parentSource = macro.ParentNode.ParentNode as MacroStatement;
			if(parentSource == null || 
				(parentSource.Name.Equals("source",StringComparison.InvariantCultureIgnoreCase) == false) &&
				(parentSource.Name.Equals("destination",StringComparison.InvariantCultureIgnoreCase) == false))
			{
				Errors.Add(new CompilerError(macro.LexicalInfo,"A command statement can appear only under a source or destination elements",null));
				return null;
			}
			if(macro.Block.Statements.Count!=1)
			{
				Errors.Add(new CompilerError(macro.LexicalInfo,"A command must contains exactly one string expression", null));
				return null;
			}
			ExpressionStatement statement = macro.Block.Statements[0] as ExpressionStatement;
			if(statement == null || (statement.Expression is StringLiteralExpression) == false)
			{
				Errors.Add(new CompilerError(macro.LexicalInfo,"A command must contain a single string expression", null));
				return null;
			}
			parentSource[Key] = statement.Expression;
			return null;
		}
	}
}