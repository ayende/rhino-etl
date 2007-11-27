using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Rhino.ETL.Impl
{
	public class BaseDataElementMacro<TElement> : AbstractNamedMacro
	{
		protected  MethodInvocationExpression methodInvocationExpression;

		public override Statement Expand(MacroStatement macro)
		{
			if (ValidateHasName(macro) == false)
				return null;
			methodInvocationExpression = new MethodInvocationExpression(
				AstUtil.CreateReferenceExpression(typeof(TElement).FullName)
				);
			if(MacroArgumentsToCreateNamedArguments(methodInvocationExpression, macro)==false)
				return null;
			methodInvocationExpression.Arguments.Add(GetNameExpression(macro));
			AddNamedArgument(methodInvocationExpression, macro, "Command", CommandMacro.Key);
			AddNamedArgument(methodInvocationExpression, macro, "CommandGenerator", CommandGeneratorMacro.Key);
			InternalLocal internalLocal = CodeBuilder.DeclareLocal(CodeHelper.GetMethod(macro.Block), "source",
																   TypeSystemServices.Map(typeof(TElement)));
			ReferenceExpression localRef = CodeBuilder.CreateLocalReference("sourceReference", internalLocal);
			macro.Block.Insert(0,
			                   new BinaryExpression(
			                   	BinaryOperatorType.Assign,
			                   	localRef,
			                   	methodInvocationExpression)
				);
			ReplaceParametersMethodSource<TElement> source = new ReplaceParametersMethodSource<TElement>(localRef);
			source.Initialize(Context);
			source.Visit(macro.Block);

			ReplaceMethodTargetAndAddParameter replaceMethodTargetAndAddParameter = new ReplaceMethodTargetAndAddParameter(localRef);
			replaceMethodTargetAndAddParameter.Initialize(Context);
			replaceMethodTargetAndAddParameter.Visit(macro.Block);
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