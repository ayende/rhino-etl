using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;

namespace Rhino.ETL.Impl
{
	public class JoinMacro : AbstractNamedMacro
	{
		public override Statement Expand(MacroStatement macro)
		{
			if (ValidateHasName(macro) == false)
				return null;
			ClassDefinition definition = new ClassDefinition();
			definition.BaseTypes.Add(CodeBuilder.CreateTypeReference(typeof(Join)));
			definition.Name = "Join_" + GetName(macro);
            
			Method apply = new Method("DoApply");
			apply.Parameters.Add(
				new ParameterDeclaration("Row",
				                         CodeBuilder.CreateTypeReference(typeof(Row))));
			apply.Parameters.Add(
				new ParameterDeclaration("Left",
				                         CodeBuilder.CreateTypeReference(typeof(Row))));
			apply.Parameters.Add(
				new ParameterDeclaration("Right",
				                         CodeBuilder.CreateTypeReference(typeof(Row))));
			definition.Members.Add(apply);

			if(macro.Block.Statements.Count!=1 || 
				!(macro.Block.Statements[0] is IfStatement))
			{
				Errors.Add(new CompilerError(macro.LexicalInfo, "Join macro can only contain an if statement",null));
				return null;
			}
			IfStatement ifStatement = (IfStatement)macro.Block.Statements[0];
			
			apply.Body = ifStatement.TrueBlock;
			if(ifStatement.FalseBlock!=null)
			{
				Errors.Add(new CompilerError(macro.LexicalInfo, "Join macro can if statement cannot contains an else",null));
				return null;
			}

			Block condition = new Block();
			condition.Add(new ReturnStatement(ifStatement.Condition));

			GetModule(macro).Members.Add(definition);

			Constructor ctor = new Constructor();
			MethodInvocationExpression callBaseCtor = 
				new MethodInvocationExpression(new SuperLiteralExpression());
			callBaseCtor.Arguments.Add(GetNameExpression(macro));
			ctor.Body.Add(callBaseCtor);
			definition.Members.Add(ctor);
			MethodInvocationExpression create = new MethodInvocationExpression(
				AstUtil.CreateReferenceExpression(definition.FullName));
			BlockExpression conditionCallable = new BlockExpression(condition);
			conditionCallable.Parameters.Add(new ParameterDeclaration("Left",
				CodeBuilder.CreateTypeReference(typeof(Row))));
			conditionCallable.Parameters.Add(new ParameterDeclaration("Right",
				CodeBuilder.CreateTypeReference(typeof(Row))));
			create.NamedArguments.Add(
				new ExpressionPair(
					new ReferenceExpression("Condition"),
					conditionCallable
					)
				);

			return new ExpressionStatement(create);
		}
	}
}