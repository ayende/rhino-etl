using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Boo.Lang;
using MbUnit.Framework;
using Rhino.ETL.Exceptions;

namespace Rhino.ETL.Tests.Sources
{
    [TestFixture]
	public class SourcesFixture : BaseTest
    {
        private EtlConfigurationContext configurationContext;

        [SetUp]
        public void TestInitialize()
        {
            configurationContext = BuildContext(@"Sources\source_only.retl");
        }

        [Test]
        public void SourceStatement_ConfiguredFromDSL_CanSetCommand()
        {
            string actual = configurationContext.Sources["Northwind"].Command;
            string expected = "SELECT * FROM Customers";
            Assert.AreEqual(expected, actual);
        }

		[Test]
		public void SourceStatement_ConfiguredFromDSL_CanUseCommandGenerator()
		{
			string actual = configurationContext.Sources["WithGenerator"].Command;
			string expected = "SELECT * FROM Customers WHERE Date >= '"+DateTime.Today +"' ";
			Assert.AreEqual(expected, actual);
		}

    	[Test]
    	public void Source_WhenSpecifyingSimpleResult_WillCreateParameterResulst()
    	{
			object actual = configurationContext.Sources["Northwind"].GetParameterValue("lastUpdate");
    		Assert.AreEqual(DateTime.Today.AddDays(-1), actual );
    	}

    	[Test]
    	public void Source_WhenEvaluatingParametersAgain_WillBeReEvaluated()
    	{
    		DataSource northwind = configurationContext.Sources["Northwind"];
    		Assert.AreNotEqual(
				northwind.GetParameterValue("ChangingParameter"),
				northwind.GetParameterValue("ChangingParameter"));
    	}

    	[Test]
    	public void Source_Parameters_WillGetCommand()
    	{
			DataSource northwind = configurationContext.Sources["Northwind"];
    		ICommandWithResult currentTime = northwind.GetParameterValue("CurrentTime") as ICommandWithResult;
			Assert.IsNotNull(currentTime);
    	}

    	[Test]
    	public void Sources_ComplexParameters_CanUseNormalControlStructures()
    	{
    		DataSource northwind = configurationContext.Sources["Northwind"];
    		bool alreadyCalled = (bool)northwind.GetParameterValue("ExplicitCallable");
    		Assert.IsFalse(alreadyCalled);
    		alreadyCalled = (bool)northwind.GetParameterValue("ExplicitCallable");
    		Assert.IsTrue(alreadyCalled);
    	}

    	[Test]
		[ExpectedException(typeof(DuplicateKeyException),"[Source Northwind] already has a parameter called 'test'")]	
    	public void AddingDuplicatedParamaters_WillThrow()
    	{
			DataSource northwind = configurationContext.Sources["Northwind"];
    		northwind.AddParameter("test", null);
			northwind.AddParameter("test", null);
    	}

    	[Test]
    	public void Source_WithComplexGenerator()
    	{
			DataSource generator = configurationContext.Sources["ComplexGenerator"];
			Environment.SetEnvironmentVariable("production", null);
			Assert.AreEqual("SELECT * FROM Test.Customers", generator.Command);
			Environment.SetEnvironmentVariable("production", "true");
			Assert.AreEqual("SELECT * FROM Production.Customers", generator.Command);
    	}

    	[Test]
    	public void Source_CanSpecifyRequiredConnection()
    	{
			DataSource generator = configurationContext.Sources["Northwind"];
			Assert.AreEqual("NorthwindConnection", generator.Connection);
    	}

    	[Test]
    	public void Source_ConnectionName_DefaultToSourceName()
    	{
			DataSource generator = configurationContext.Sources["ComplexGenerator"];
			Assert.AreEqual(generator.Name, generator.Connection);
    
    	}
    }
}
