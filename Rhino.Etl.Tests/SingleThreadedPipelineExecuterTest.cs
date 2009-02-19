using System.Collections.Generic;
using MbUnit.Framework;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;
using Rhino.Etl.Core.Pipelines;
using Rhino.Mocks;

namespace Rhino.Etl.Tests
{
    [TestFixture]
    public class SingleThreadedPipelineExecuterTest
    {
        [Test]
        public void Should_not_execute_operations_twice()
        {
            var mockery = new MockRepository();

            using (var stubProcess = mockery.Stub<EtlProcess>())
            {
                stubProcess.PipelineExecuter = new SingleThreadedPipelineExecuter();

                var spyOperation = new SpyOperation();
                stubProcess.Register(spyOperation);
                stubProcess.Register(new OperationWhichIteratesTwiceItsInput());

                stubProcess.Execute();

                Assert.AreEqual(1, spyOperation.Enumerations);
            }
        }

        class SpyOperation : AbstractOperation
        {
            public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
            {
                Enumerations++;
                yield break;
            }

            public int Enumerations { get; set; }
        }

        class OperationWhichIteratesTwiceItsInput : AbstractOperation
        {
            public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
            {
                foreach (var row in rows) ;
                foreach (var row in rows) ;
                yield break;
            }
        }
    }
}