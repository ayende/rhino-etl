using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;
using Rhino.Etl.Core.Pipelines;
using Rhino.Etl.Tests.Joins;
using Rhino.Mocks;

namespace Rhino.Etl.Tests
{
    
    public class SingleThreadedPipelineExecuterTest
    {
        [Fact]
        public void OperationsAreExecutedOnce()
        {
            var iterations = 0;
            
            using (var stubProcess = MockRepository.GenerateStub<EtlProcess>())
            {
            	stubProcess.Stub(x => x.TranslateRows(null)).IgnoreArguments()
            		.WhenCalled(x => x.ReturnValue = x.Arguments[0]);

                stubProcess.PipelineExecuter = new SingleThreadedPipelineExecuter();

                stubProcess.Register(new InputSpyOperation(() => iterations++));
                stubProcess.Register(new OutputSpyOperation(2));

                stubProcess.Execute();
            }

            Assert.Equal(1, iterations);
        }

        [Fact]
        public void MultipleIterationsYieldSameResults()
        {
            var accumulator = new ArrayList();

            using (var process = MockRepository.GenerateStub<EtlProcess>())
            {
				process.Stub(x => x.TranslateRows(null)).IgnoreArguments()
					.WhenCalled(x => x.ReturnValue = x.Arguments[0]);

                process.PipelineExecuter = new SingleThreadedPipelineExecuter();

                process.Register(new GenericEnumerableOperation(new[] {Row.FromObject(new {Prop = "Hello"})}));
                process.Register(new OutputSpyOperation(2, r => accumulator.Add(r["Prop"])));

                process.Execute();
            }

            Assert.Equal(accumulator.Cast<string>().ToArray(), Enumerable.Repeat("Hello", 2).ToArray());
        }

        class InputSpyOperation : AbstractOperation
        {
            private readonly Action onExecute;

            public InputSpyOperation(Action onExecute)
            {
                this.onExecute = onExecute;
            }

            public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
            {
                onExecute();
                yield break;
            }
        }

        class OutputSpyOperation : AbstractOperation
        {
            private readonly int numberOfIterations;
            private readonly Action<Row> onRow;

            public OutputSpyOperation(int numberOfIterations) : this(numberOfIterations, r => {})
            {}

            public OutputSpyOperation(int numberOfIterations, Action<Row> onRow)
            {
                this.numberOfIterations = numberOfIterations;
                this.onRow = onRow;
            }

            public override IEnumerable<Row> Execute(IEnumerable<Row> rows)
            {
                for (var i = 0; i < numberOfIterations; i++)
                    foreach (var row in rows)
                        onRow(row);

                yield break;
            }
        }
    }
}