using System;
using System.Collections.Generic;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;
using Rhino.Etl.Core.Pipelines;
using Xunit;

namespace Rhino.Etl.Tests
{
    public class PipelineEventsFixture
    {

        [Fact]
        public void    RaiseEventsWhenPipelineExecuted()
        {
            //Arrange
            var    startingCalled = 0;
            var    completingCalled = 0;
            var    pipeline = new TestPipelineExecuter();
            pipeline.NotifyExecutionStarting +=    delegate { startingCalled += 1;    };
            pipeline.NotifyExecutionCompleting += delegate { completingCalled += 1;    };

            //Act
            pipeline.Execute("Test", new IOperation[0],    rows =>    rows);

            //Assert);
            Assert.Equal(1,    startingCalled);
            Assert.Equal(1,    completingCalled);
        }
    }

    public class TestPipelineExecuter :    AbstractPipelineExecuter
    {
        protected override IEnumerable<Row>    DecorateEnumerableForExecution(IOperation operation, IEnumerable<Row> enumerator)
        {
            throw new NotImplementedException();
        }
    }
}
