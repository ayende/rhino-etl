using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;

namespace Rhino.ETL.Impl
{
	public class TransfromGeneratorExpressionToBlocks : AbstractNamespaceSensitiveTransformerCompilerStep
	{
		public override void Run()
		{
			Visit(CompileUnit);
		}

		public override void OnBinaryExpression(BinaryExpression node)
		{
			if(node.Operator!=BinaryOperatorType.Assign)
				return;
			node.Right = GetNodeToReplace(node.Left, node.Right);
		}

		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			foreach (ExpressionPair pair in node.NamedArguments)
			{
				pair.Second = GetNodeToReplace(pair.First, pair.Second);
			}
		}

		private static Expression GetNodeToReplace(Expression first, Expression second)
		{
			ReferenceExpression left = first as ReferenceExpression;
			if(left==null)
				return second;
			if(left.Name.EndsWith("Generator"))
			{
				BlockExpression expression = second as BlockExpression;
				if(expression==null)
				{
					expression = new BlockExpression();
					expression.Body.Add(new ReturnStatement(second));
				}
				return  expression;
			}
			return second;
		}
	}
}
