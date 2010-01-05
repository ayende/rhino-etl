namespace Rhino.Etl.Tests.Dsl
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Aggregation;
    using Core;
    using Joins;
    using Xunit;
    using Rhino.Etl.Core.Operations;
    using Rhino.Etl.Dsl;

    
    public class WireEtlProcessEventsFixture : BaseAggregationFixture
    {
        [Fact]
        public void CanCompileWithRowProcessedEvent()
        {
            using (EtlProcess process = CreateDslInstance("Dsl/WireRowProcessedEvent.boo"))
                Assert.NotNull(process);    
        }

        [Fact]
        public void CheckIfOnRowProcessedEventWasWired()
        {
            using (var process = CreateDslInstance("Dsl/WireRowProcessedEvent.boo"))
            {
                process.Register(new GenericEnumerableOperation(rows));
                ResultsToList operation = new ResultsToList();
                process.RegisterLast(operation);
                process.Execute();
                Assert.Equal(1, operation.Results.Count);
                Assert.Equal("chocolate, sugar, coffee", operation.Results[0]["result"]);
            }
        }

        [Fact]
        public void CanCompileWithFinishedProcessingEvent()
        {
            using (var process = CreateDslInstance("Dsl/WireOnFinishedProcessingEvent.boo"))
                Assert.NotNull(process);
        }

        [Fact]
        public void CheckIfOnFinishedProcessingEventWasWired()
        {
            using (var process = CreateDslInstance("Dsl/WireOnFinishedProcessingEvent.boo"))
            {
                process.Register(new GenericEnumerableOperation(rows));
                ResultsToList operation = new ResultsToList();
                process.RegisterLast(operation);
                process.Execute();
                Assert.Equal(1, operation.Results.Count);
                Assert.True(File.Exists(@"OnFinishedProcessing.wired"));

                File.Delete(@"OnFinishedProcessing.wired");
            }

        }
    }
}
