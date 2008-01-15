namespace Rhino.Etl.Tests.Dsl
{
	using System;
	using System.Collections.Generic;
    using System.IO;
    using Aggregation;
    using Core;
    using Joins;
    using MbUnit.Framework;
    using Rhino.Etl.Core.Operations;
    using Rhino.Etl.Dsl;

    [TestFixture]
    public class AggregationFixture : BaseAggregationFixture
    {
        [Test]
        public void CanCompile()
        {
            EtlProcess process = CreateDslInstance("Dsl/Aggregate.boo");
            Assert.IsNotNull(process);
        }


    	[Test]
        public void CanPerformAggregationFromDsl()
        {
            EtlProcess process = CreateDslInstance("Dsl/Aggregate.boo");
            process.Register(new GenericEnumerableOperation(rows));
            ResultsToList operation = new ResultsToList();
            process.RegisterLast(operation);
            process.Execute();
            Assert.AreEqual(1, operation.Results.Count);
            Assert.AreEqual("milk, sugar, coffee", operation.Results[0]["result"]);
        }
    }
}