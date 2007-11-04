using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;

namespace Rhino.ETL.Impl
{
	public class OnCompleteMacro : AbstractAstMacro
	{
		public static readonly object Key = new object();

		public override Statement Expand(MacroStatement macro)
		{
			MacroStatement parentSource = macro.ParentNode.ParentNode as MacroStatement;
			if (parentSource == null ||
			    (parentSource.Name.Equals("transform", StringComparison.InvariantCultureIgnoreCase) == false))
			{
				Errors.Add(new CompilerError(macro.LexicalInfo, "An OnComplete statement can appear only under a transform element", null));
				return null;
			}
			parentSource[Key] = macro.Block;
			return null;
		}
	}
}