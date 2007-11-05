using System;
using System.Collections.Generic;
using System.Text;
using Boo.Lang.Compiler;
using MbUnit.Framework;
using System.IO;

namespace Rhino.ETL.Tests.Errors
{
	using Engine;

	[TestFixture]
	public class ErrorsFixture : BaseTest
	{
		public void Evaluate(string file, string exepctedMessage)
		{
			try
			{
				BuildContext(Path.Combine("Errors", file));
				Assert.Fail("Expected exception to occur with message: "+exepctedMessage);
			}
			catch (Exception e)
			{
				Assert.Contains(e.Message, exepctedMessage);
			}	
		}

		[Test]
		public void CommandAsRoot()
		{
			Evaluate("command_as_root.retl",
				"A command statement can appear only under a source or destination elements");
		}

		[Test]
		public void CommandContainsNonStringExpression()
		{
			Evaluate("command_contains_non_string_expression.retl",
				"A command must contain a single string expression");
		}

		[Test]
		public void CommandMustContainsSingleExpression()
		{
			Evaluate("command_with_more_than_single_statement.retl",
				"A command must contains exactly one string expression");
		}

		[Test]
		public void ScriptThatThrows()
		{
			Evaluate("script_throw_exception.retl",
				@"script_throw_exception has thrown an exception when evaluated: Object reference not set to an instance of an object");
		}

		[Test]
		public void SourceWithNoName()
		{
			Evaluate("source_no_name.retl",
                @"SourceMacro must have a name");
		
		}

        [Test]
        public void TransformWithNoName()
        {
            Evaluate("transform_no_name.retl",
                @"TransformMacro must have a name");
        }

		[Test]
		public void WhenComponentThrowsInTarget_WillReturnErrorResult()
		{
			EtlConfigurationContext context = BuildContext(@"Errors\target_with_throwing_component.retl");
			ExecutionPackage package = context.BuildPackage();
			ExecutionResult result = package.Execute("default");
			Assert.AreEqual(ExecutionStatus.Failure, result.Status);
			Assert.LowerEqualThan(1, result.Exceptions.Count);
			Assert.AreEqual("Just an error", result.Exceptions[0].InnerException.Message);
		}

		[Test]
		public void WhenComponentThrowsInTarget_WillNotExecuteFurtherTasks()
		{
			bool copyUserCompeleted = false;
			EtlConfigurationContext context = BuildContext(@"Errors\target_with_throwing_component.retl");
			context.Pipelines["CopyUsers"].Completed += delegate { copyUserCompeleted = true; };

			ExecutionPackage package = context.BuildPackage();
			ExecutionResult result = package.Execute("default");
			Assert.AreEqual(ExecutionStatus.Failure, result.Status);
			Assert.IsFalse(copyUserCompeleted);
		}
	}
}
