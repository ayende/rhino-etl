using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using Rhino.ETL.Exceptions;

namespace Rhino.ETL.Tests
{
	[TestFixture]
	public class ContextfulObjectFixture : ContextfulObjectBase<ContextfulObjectFixture>
	{
		public override string Name
		{
			get { return "TheTest"; }
		}

		[Test]
		public void ContextfulObject_HasLogger_DefinedForCurrentImplementation()
		{
			Assert.AreEqual(typeof(ContextfulObjectFixture).FullName, Logger.Logger.Name);
		}

		[Test]
		[ExpectedException(typeof(ContextException), "You are not inside context for ContextfulObjectFixture")]
		public void AccessingContext_WhenNoContextDefined_WillThrow()
		{
			ContextfulObjectFixture fake = ContextfulObjectFixture.Current;
		}

		[Test]
		[ExpectedException(typeof(ContextException), "Tried to set the Current context for ContextfulObjectFixture without first clearing the existing one!")]
		public void EnterringContext_WhenAlreadyInOne_ShouldThrow()
		{
			using(EnterContext())
			{
				new ContextfulObjectFixture().EnterContext();
			}
		}
	}
}
