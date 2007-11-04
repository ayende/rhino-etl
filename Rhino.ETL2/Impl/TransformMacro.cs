using Boo.Lang.Compiler.Ast;

namespace Rhino.ETL.Impl
{
	using Engine;

	public class TransformMacro : AbstractNamedMacro
    {
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
            definition.Members.Add(apply);

    		Block onCompleteBlock = (Block)macro[OnCompleteMacro.Key];
    		if(onCompleteBlock!=null)
    		{
    			Method onComplete = new Method("OnComplete");
				onComplete.Parameters.Add(new ParameterDeclaration("QueueName",
					CodeBuilder.CreateTypeReference(typeof(string))));
    			onComplete.Body = onCompleteBlock;
				definition.Members.Add(onComplete);
    		}

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
