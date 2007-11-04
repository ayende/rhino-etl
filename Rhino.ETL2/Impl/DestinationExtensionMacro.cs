using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;

namespace Rhino.ETL.Impl
{
	public class DestinationExtensionMacro : AbstractAstMacro
	{
		public override Statement Expand(MacroStatement macro)
		{
			MacroStatement parentSource = macro.ParentNode.ParentNode as MacroStatement;
			if (parentSource == null ||
			    (parentSource.Name.Equals("destination", StringComparison.InvariantCultureIgnoreCase) == false))
			{
				Errors.Add(new CompilerError(macro.LexicalInfo, "A " + GetType().Name + " statement can appear only under a destination element", null));
				return null;
			}
			parentSource[GetType()] = macro.Block;
			return null;
		}
	}

	public class InitializeMacro : DestinationExtensionMacro
	{
		
	}

	public class OnRowMacro : DestinationExtensionMacro
	{
		
	}

	public class CleanUpMacro : DestinationExtensionMacro
	{
		
	}
}