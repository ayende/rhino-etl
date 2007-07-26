using System;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.TypeSystem;

namespace Rhino.ETL.Impl
{
	public class PipelineMacro : AbstractNamedMacro
	{
		private static string[] associationTypeNames = Enum.GetNames(typeof (AssoicationType));

		public override Statement Expand(MacroStatement macro)
		{
			if (ValidateHasName(macro) == false)
				return null;
			Method method = CodeHelper.GetMethod(macro.Block);
			Block block = new Block();
			string pipelineVar = AddCreatePipeline(block, macro, method);
			foreach (Statement statement in macro.Block.Statements)
			{
				BinaryExpression expr = NeedSpecialProcessing(block, statement);
				if (expr == null)
					continue;

				ReferenceExpression left = expr.Left as ReferenceExpression;
				if (left == null ||
				    (expr.Right is ReferenceExpression) == false &&
				    (expr.Right is MethodInvocationExpression) == false)
				{
					Errors.Add(
						new CompilerError(expr.LexicalInfo,
						                  "Invalid pipeline statement (Expected structure is 'Source >> Transform(Param: Value)'",
						                  null));
					return null;
				}

				MethodInvocationExpression createAssociation =
					new MethodInvocationExpression(AstUtil.CreateReferenceExpression(typeof (PipelineAssociation).FullName));
				InternalLocal associationLocal =
					CodeBuilder.DeclareTempLocal(method, TypeSystemServices.Map(typeof (PipelineAssociation)));
				block.Add(new BinaryExpression(BinaryOperatorType.Assign,
				                               new ReferenceExpression(associationLocal.Name),
				                               createAssociation
				          	));
				SetAssociationProperties(block, left,
				                         associationLocal.Name + ".FromType",
				                         associationLocal.Name + ".From",
				                         associationLocal.Name + ".ToQueue");
				ReferenceExpression right;
				if (expr.Right is ReferenceExpression)
				{
					right = (ReferenceExpression) expr.Right;
				}
				else
				{
					MethodInvocationExpression mie = (MethodInvocationExpression) expr.Right;
					if (mie.Arguments.Count > 0)
					{
						Errors.Add(new CompilerError(expr.Right, "You can only use named arguments on a pipeline.", null));
						return null;
					}
					ReferenceExpression set = AstUtil.CreateReferenceExpression(associationLocal.Name + ".Parameters.set_Item");
					foreach (ExpressionPair pair in mie.NamedArguments)
					{
						block.Add(
							new MethodInvocationExpression(set,
							                               new StringLiteralExpression(((ReferenceExpression) pair.First).Name),
							                               pair.Second
								)
							);
					}
					right = (ReferenceExpression) mie.Target;
				}
				SetAssociationProperties(block, right,
				                         associationLocal.Name + ".ToType",
				                         associationLocal.Name + ".To",
				                         associationLocal.Name + ".FromQueue");

				block.Add(
					new MethodInvocationExpression(AstUtil.CreateReferenceExpression(pipelineVar + ".AddAssociation"),
					                               new ReferenceExpression(associationLocal.Name))
					);
			}
			return block;
		}

		private static void SetAssociationProperties(Block block,
		                                             ReferenceExpression referenceExpression,
		                                             string associationType,
		                                             string property, string queue)
		{
			MemberReferenceExpression mre = referenceExpression as MemberReferenceExpression;
			if (mre != null)
			{
				string name = GetRootName(mre);
				//use to disambigue the source, so we need to set the Type of the association
				if (Array.IndexOf(associationTypeNames, name) != -1)
				{
					block.Add(
						new BinaryExpression(BinaryOperatorType.Assign,
						                     AstUtil.CreateReferenceExpression(associationType),
						                     AstUtil.CreateReferenceExpression(
						                     	string.Format("{0}.{1}",
						                     	              typeof (AssoicationType).FullName,
						                     	              name))
							)
						);
					referenceExpression = RemoveRootElement(mre);
					mre = referenceExpression as MemberReferenceExpression;
				}
			}
			if (mre != null)
			{
				Expression target = mre.Target;
				if (target is ReferenceExpression)
				{
					target = new StringLiteralExpression(((ReferenceExpression) target).Name);
				}
				block.Add(
					new BinaryExpression(BinaryOperatorType.Assign,
					                     AstUtil.CreateReferenceExpression(property),
					                     target
						)
					);
				block.Add(
					new BinaryExpression(BinaryOperatorType.Assign,
					                     AstUtil.CreateReferenceExpression(queue),
					                     new StringLiteralExpression(mre.Name)
						)
					);
			}
			else
			{
				block.Add(
					new BinaryExpression(BinaryOperatorType.Assign,
					                     AstUtil.CreateReferenceExpression(property),
					                     new StringLiteralExpression(referenceExpression.Name)
						)
					);
			}
		}

		private static ReferenceExpression RemoveRootElement(MemberReferenceExpression mre)
		{
			if (mre.Target is MemberReferenceExpression)
			{
				return new MemberReferenceExpression(
					new ReferenceExpression(((ReferenceExpression) mre.Target).Name),
					mre.Name);
			}
			return new ReferenceExpression(mre.Name);
		}

		private static string GetRootName(MemberReferenceExpression mre)
		{
			if (mre.Target is MemberReferenceExpression)
				return GetRootName((MemberReferenceExpression) mre.Target);
			return ((ReferenceExpression) mre.Target).Name;
		}

		private static BinaryExpression NeedSpecialProcessing(Block block, Statement statement)
		{
			ExpressionStatement expressionStatement = statement as ExpressionStatement;
			if (expressionStatement == null ||
			    (expressionStatement.Expression is BinaryExpression) == false)
			{
				block.Add(statement);
				return null;
			}
			BinaryExpression expr = (BinaryExpression) expressionStatement.Expression;
			if (expr.Operator != BinaryOperatorType.ShiftRight)
			{
				block.Add(statement);
				return null;
			}
			return expr;
		}

		private string AddCreatePipeline(Block block, MacroStatement macro, Method method)
		{
			MethodInvocationExpression create =
				new MethodInvocationExpression(AstUtil.CreateReferenceExpression(typeof (Pipeline).FullName));
			create.Arguments.Add(GetNameExpression(macro));
			InternalLocal local = CodeBuilder.DeclareTempLocal(method, TypeSystemServices.Map(typeof (Pipeline)));
			block.Add(new BinaryExpression(BinaryOperatorType.Assign,
			                               new ReferenceExpression(local.Name),
			                               create
			          	));
			return local.Name;
		}
	}
}