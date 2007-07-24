using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;
using Boo.Lang.Compiler.TypeSystem;

namespace Rhino.ETL.Impl
{
	public class BaseDataElementMacro<TElement> : AbstractNamedMacro
	{
		public override Statement Expand(MacroStatement macro)
		{
			if (ValidateHasName(macro) == false)
				return null;
			MethodInvocationExpression create = new MethodInvocationExpression(
				AstUtil.CreateReferenceExpression(typeof(TElement).FullName)
				);
			foreach (Expression argument in macro.Arguments)
			{
				if (argument == macro.Arguments[0])
					continue;
				BinaryExpression expression = argument as BinaryExpression;
				if(expression ==null)
				{
					Errors.Add(new CompilerError(macro.LexicalInfo,
					                             GetType().Name + @" parameters much be in the format of Connection = 'ConnectionName'",null));
					return null;
				}
				create.NamedArguments.Add(
					new ExpressionPair(expression.Left, expression.Right)
					);
			}
			create.Arguments.Add(GetNameExpression(macro));
			AddNamedArgument(create, macro, "Command", CommandMacro.Key);
			AddNamedArgument(create, macro, "CommandGenerator", CommandGeneratorMacro.Key);
			InternalLocal internalLocal = CodeBuilder.DeclareLocal(CodeHelper.GetMethod(macro.Block), "source",
																   TypeSystemServices.Map(typeof(TElement)));
			ReferenceExpression localRef = CodeBuilder.CreateLocalReference("sourceReference", internalLocal);
			macro.Block.Insert(0,
			                   new BinaryExpression(
			                   	BinaryOperatorType.Assign,
			                   	localRef,
			                   	create)
				);
			ReplaceParametersMethodSource<TElement> source = new ReplaceParametersMethodSource<TElement>(localRef);
			source.Initialize(Context);
			source.Visit(macro.Block);
			return macro.Block;
		}

		private static void AddNamedArgument(MethodInvocationExpression createDataSource, MacroStatement macro, string name,
		                                     object key)
		{
			Expression expression = (Expression) macro[key];
			if (expression == null)
				return;
			ExpressionPair pair = new ExpressionPair(new ReferenceExpression(name),
			                                         expression);
			createDataSource.NamedArguments.Add(pair);
		}
	}
}