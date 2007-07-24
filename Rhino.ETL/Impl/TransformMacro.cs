using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Boo.Lang;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.TypeSystem;
using Rhino.ETL.Exceptions;

namespace Rhino.ETL.Impl
{
    public class TransformMacro : AbstractNamedMacro
    {
    	private Block blockToCheckBeforeProcessMethodBodies;

    	public override Statement Expand(MacroStatement macro)
        {
            if (ValidateHasName(macro) == false)
                return null;
            ClassDefinition definition = new ClassDefinition();
            definition.BaseTypes.Add(CodeBuilder.CreateTypeReference(typeof(Transform)));
            definition.Name = "Transform_" + GetName(macro);
            
            Method apply = new Method("DoApply");
            apply.Parameters.Add(
                new ParameterDeclaration("Row",
                CodeBuilder.CreateTypeReference(typeof(Row))));
			apply.Parameters.Add(
				new ParameterDeclaration("Parameters",
				CodeBuilder.CreateTypeReference(typeof(QuackingDictionary))));
            apply.Body = macro.Block;
    		this.blockToCheckBeforeProcessMethodBodies = apply.Body;
            definition.Members.Add(apply);

            GetModule(macro).Members.Add(definition);

            Constructor ctor = new Constructor();
            MethodInvocationExpression callBaseCtor = 
                new MethodInvocationExpression(new SuperLiteralExpression());
            callBaseCtor.Arguments.Add(GetNameExpression(macro));
            ctor.Body.Add(callBaseCtor);
            definition.Members.Add(ctor);
            MethodInvocationExpression create = new MethodInvocationExpression(
                AstUtil.CreateReferenceExpression(definition.FullName));

            return new ExpressionStatement(create);
        }
    }
}
