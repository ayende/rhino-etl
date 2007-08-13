using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;

namespace Rhino.ETL.Impl
{
	public class DestinationMacro : BaseDataElementMacro<DataDestination>
	{
		public override Statement Expand(MacroStatement macro)
		{
			Statement statement = base.Expand(macro);

			Block init = (Block)macro[typeof(InitializeMacro)];
			if (init != null)
			{
				BlockExpression be = new BlockExpression(init);
				be.Parameters.Add(new ParameterDeclaration("Parameters",
														   CodeBuilder.CreateTypeReference(typeof(QuackingDictionary))));
				methodInvocationExpression.NamedArguments.Add(
					new ExpressionPair(new ReferenceExpression("InitializeBlock"), be)
					);
			}


			Block onRow = (Block)macro[typeof(OnRowMacro)];
			if (onRow != null)
			{
				BlockExpression be = new BlockExpression(onRow);
				be.Parameters.Add(new ParameterDeclaration("Parameters",
														   CodeBuilder.CreateTypeReference(typeof(QuackingDictionary))));
				be.Parameters.Add(new ParameterDeclaration("Row",
										   CodeBuilder.CreateTypeReference(typeof(Row))));

				methodInvocationExpression.NamedArguments.Add(
					new ExpressionPair(new ReferenceExpression("OnRowBlock"), be)
					);
			}

			Block cleanup = (Block)macro[typeof(CleanUpMacro)];
			if (cleanup != null)
			{
				BlockExpression be = new BlockExpression(cleanup);
				be.Parameters.Add(new ParameterDeclaration("Parameters",
														   CodeBuilder.CreateTypeReference(typeof(QuackingDictionary))));
				methodInvocationExpression.NamedArguments.Add(
					new ExpressionPair(new ReferenceExpression("CleanUpBlock"), be)
					);
			}
			return statement;
		}
	}
}