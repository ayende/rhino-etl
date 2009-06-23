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
    public class WireEtlProcessEventsFixture : BaseAggregationFixture
    {
        [Test]
        public void CanCompileWithRowProcessedEvent()
        {
            using (EtlProcess process = CreateDslInstance("Dsl/WireRowProcessedEvent.boo"))
                Assert.IsNotNull(process);    
        }

        [Test]
        public void CheckIfOnRowProcessedEventWasWired()
        {
            using (var process = CreateDslInstance("Dsl/WireRowProcessedEvent.boo"))
            {
                process.Register(new GenericEnumerableOperation(rows));
                ResultsToList operation = new ResultsToList();
                process.RegisterLast(operation);
                process.Execute();
                Assert.AreEqual(1, operation.Results.Count);
                Assert.AreEqual("chocolate, sugar, coffee", operation.Results[0]["result"]);
            }
        }

        [Test]
        public void CanCompileWithFinishedProcessingEvent()
        {
            using (var process = CreateDslInstance("Dsl/WireOnFinishedProcessingEvent.boo"))
                Assert.IsNotNull(process);
        }

        [Test]
        public void CheckIfOnFinishedProcessingEventWasWired()
        {
            using (var process = CreateDslInstance("Dsl/WireOnFinishedProcessingEvent.boo"))
            {
                process.Register(new GenericEnumerableOperation(rows));
                ResultsToList operation = new ResultsToList();
                process.RegisterLast(operation);
                process.Execute();
                Assert.AreEqual(1, operation.Results.Count);
                Assert.IsTrue(File.Exists(@"OnFinishedProcessing.wired"));

                File.Delete(@"OnFinishedProcessing.wired");
            }

        }
    }
}
