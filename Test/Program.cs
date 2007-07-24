using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Boo.Lang.Compiler;
using Boo.Lang.Compiler.Ast;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;
using Boo.Lang.Compiler.Steps;

namespace Test
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				string code = @"
def AddParam(name as string, obj):
	print '${name} = ${obj}'
@foo = date.Now
";

				BooCompiler compiler = new BooCompiler();
				compiler.Parameters.Ducky = true;
				compiler.Parameters.Pipeline = new CompileToMemory();
				compiler.Parameters.OutputType = CompilerOutputType.Library;
				compiler.Parameters.Input.Add(new StringInput("test",code));
				compiler.Parameters.Pipeline.Insert(2, new TransformAssignToParameters());
				compiler.Parameters.References.Add(Assembly.GetExecutingAssembly());
				CompilerContext run = compiler.Run();
				if (run.Errors.Count != 0)
				{
					throw new CompilerError(string.Format("Compilation error! {0}", run.Errors.ToString(true)));
				}
				run.GeneratedAssembly.EntryPoint.Invoke(null, null);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
		}
	}

	public class TransformAssignToParameters : AbstractNamespaceSensitiveTransformerCompilerStep
	{

		public override void Run()
		{
			Visit(CompileUnit);
		}

		public override void OnBinaryExpression(BinaryExpression node)
		{
			if (node.Operator != BinaryOperatorType.Assign)
				return;
			ReferenceExpression left = node.Left as ReferenceExpression;
			if (left != null && left.Name.StartsWith("@"))
			{
				MethodInvocationExpression addParameterInvocation = new MethodInvocationExpression();

				addParameterInvocation.Arguments.Add(new StringLiteralExpression(left.Name.Substring(1)));
				addParameterInvocation.Arguments.Add(node.Right);
				ReplaceCurrentNode(addParameterInvocation);
			}
		}
	}
}
