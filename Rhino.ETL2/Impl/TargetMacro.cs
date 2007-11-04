using System;
using System.Collections.Generic;
using System.Text;
using Boo.Lang.Compiler.Ast;
using Rhino.ETL.Engine;

namespace Rhino.ETL.Impl
{
	public class TargetMacro : AbstractNamedMacro
	{
		public override Statement Expand(MacroStatement macro)
		{
			if(ValidateHasName(macro)==false)
				return null;
			ClassDefinition definition = new ClassDefinition();
			definition.BaseTypes.Add(CodeBuilder.CreateTypeReference(typeof(Target)));
			definition.Name = "Target_" + GetName(macro);

			Method prepareMethod = new Method();
			prepareMethod.Name = "Prepare";
			prepareMethod.Body = macro.Block;
			definition.Members.Add(prepareMethod);

			GetModule(macro).Members.Add(definition);

			Constructor ctor = new Constructor();
			MethodInvocationExpression callBaseCtor =
				new MethodInvocationExpression(new SuperLiteralExpression());
			callBaseCtor.Arguments.Add(GetNameExpression(macro));
			ctor.Body.Add(callBaseCtor);
			definition.Members.Add(ctor);

			MethodInvocationExpression create = new MethodInvocationExpression(
			AstUtil.CreateReferenceExpression(definition.FullName));
			MacroArgumentsToCreateNamedArguments(create, macro); 
			return new ExpressionStatement(create);
		}
	}
}
