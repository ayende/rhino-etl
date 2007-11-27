using System;
using System.Diagnostics;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.Steps;

namespace Rhino.ETL.Impl
{
	public class ReplaceMethodTargetAndAddParameter : AbstractVisitorCompilerStep
	{
		private readonly ReferenceExpression referenceExpression;
		private bool currentlyInExecuteBlock;

		public ReplaceMethodTargetAndAddParameter(ReferenceExpression referenceExpression)
		{
			this.referenceExpression = referenceExpression;
		}

		public override void Run()
		{
		}

		public override void OnMethodInvocationExpression(MethodInvocationExpression node)
		{
			if ((node.Target is ReferenceExpression) == false)
				return;
			ReferenceExpression expression = (ReferenceExpression)node.Target;
			if ("Execute".Equals(expression.Name, StringComparison.InvariantCultureIgnoreCase))
			{
				node.Target = new MemberReferenceExpression(referenceExpression, expression.Name);
				if (node.Arguments.Count == 0 || (node.Arguments[0] is BlockExpression) == false)
				{
					Errors.Add(new CompilerError(node.LexicalInfo, "Execute must have a code block to execute!", null));
					return;
				}
				BlockExpression block = (BlockExpression)node.Arguments[0];
				block.Parameters.Add(new ParameterDeclaration("sourceLocalVariable", CodeBuilder.CreateTypeReference(typeof(DataSource))));
				currentlyInExecuteBlock = true;
				Visit(block.Body);
				currentlyInExecuteBlock = false;
			}
			if (currentlyInExecuteBlock && "SendRow" == expression.Name)
			{
				node.Target = new MemberReferenceExpression(new ReferenceExpression("sourceLocalVariable"),
					expression.Name);
			}
			base.OnMethodInvocationExpression(node);
		}
	}
}