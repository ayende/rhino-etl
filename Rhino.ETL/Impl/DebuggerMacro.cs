using System;
using System.Collections.Generic;
using System.Text;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;

namespace Rhino.ETL.Impl
{
	public class DebuggerMacro : AbstractAstMacro
	{
		public override Boo.Lang.Compiler.Ast.Statement Expand(Boo.Lang.Compiler.Ast.MacroStatement macro)
		{
			return new ExpressionStatement(
				new MethodInvocationExpression(AstUtil.CreateReferenceExpression("System.Diagnostics.Debugger.Break"))
				);
		}
	}
}
