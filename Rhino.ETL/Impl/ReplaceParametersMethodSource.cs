using System;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;

namespace Rhino.ETL.Impl
{
	public class ReplaceParametersMethodSource<TElement> : AbstractVisitorCompilerStep
	{
		private readonly ReferenceExpression localRef;

		public ReplaceParametersMethodSource(ReferenceExpression localRef)
		{
			this.localRef = localRef;
		}

		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			if ((node.Target is ReferenceExpression) == false)
				return;
			ReferenceExpression target = (ReferenceExpression)node.Target;
			if (target.Name != "Parameters")
				return;
			node.Target = new MemberReferenceExpression(localRef, "Parameters");
			if (node.Arguments.Count != 1)
			{
				throw new InvalidOperationException("Parameters can only have one parameter");
			}
			BlockExpression block = node.Arguments[0] as BlockExpression;
			if (block == null)
			{
				throw new InvalidOperationException("Parameters must accept a single callable parameter");
			}
			block.Parameters.Add(new ParameterDeclaration("parent",
			                                              CodeBuilder.CreateTypeReference(typeof(TElement))));
			base.OnMethodInvocationExpression(node);
		}

		public override void OnBinaryExpression(BinaryExpression node)
		{
			if (node.Operator != BinaryOperatorType.Assign)
				return;
			ReferenceExpression left = node.Left as ReferenceExpression;
			if (left == null)
				return;
			if (left.Name.StartsWith("@") == false)
				return;

			MethodInvocationExpression mie = new MethodInvocationExpression();
			mie.Target = AstUtil.CreateReferenceExpression("parent.AddParameter");
			mie.Arguments.Add(new StringLiteralExpression(
			                  	left.Name.Substring(1)
			                  	));

			BlockExpression callable = node.Right as BlockExpression;
			if (callable == null)
			{
				callable = new BlockExpression();
				callable.Body.Add(new ReturnStatement(node.Right));
			}

			mie.Arguments.Add(callable);

			node.ParentNode.Replace(node, mie);
		}

		public override void Run()
		{
		}
	}
}