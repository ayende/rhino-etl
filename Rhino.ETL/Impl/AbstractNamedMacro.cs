using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;

namespace Rhino.ETL.Impl
{
    public abstract class AbstractNamedMacro : AbstractAstMacro
    {
        protected bool ValidateHasName(MacroStatement macro)
        {
            if (macro.Arguments.Count == 0 ||
                (macro.Arguments[0] is ReferenceExpression) == false &&
                (macro.Arguments[0] is StringLiteralExpression) == false)
            {
                Errors.Add(new CompilerError(macro.LexicalInfo, GetType().Name + " must have a name", null));
                return false;
            }
            return true;
        }

        protected static string GetName(MacroStatement macro)
        {
            if (macro.Arguments[0] is StringLiteralExpression)
                return ((StringLiteralExpression) macro.Arguments[0]).Value;
            return ((ReferenceExpression) macro.Arguments[0]).Name;
        }

        protected Expression GetNameExpression(MacroStatement macro)
        {
            ReferenceExpression referenceExpression = macro.Arguments[0] as ReferenceExpression;
            if (referenceExpression != null &&
                NameResolutionService.Resolve(referenceExpression.Name) == null)
            {
                return CodeBuilder.CreateStringLiteral(referenceExpression.Name);
            }
            else
            {
                return macro.Arguments[0];
            }
        }

		protected bool MacroArgumentsToCreateNamedArguments(MethodInvocationExpression create, MacroStatement macro)
		{
			foreach (Expression argument in macro.Arguments)
			{
				if (argument == macro.Arguments[0])
					continue;
				BinaryExpression expression = argument as BinaryExpression;
				if (expression == null)
				{
					Errors.Add(new CompilerError(macro.LexicalInfo,
												 GetType().Name + @" parameters much be in the format of Connection = 'ConnectionName'", null));
					return false;
				}
				create.NamedArguments.Add(
					new ExpressionPair(expression.Left, expression.Right)
					);
			}
			return true;
		}

        protected static Module GetModule(MacroStatement macro)
        {
            Node node = macro.ParentNode;
            while ((node is Module) == false)
            {
                node = node.ParentNode;
            }
            return (Module)node;
        }
    }
}