using System;
using System.Collections.Generic;
using System.Reflection;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;
using Rhino.Mocks;
using Xunit;

namespace Rhino.Etl.Tests.Branches
{
    public class BranchEventsFixture
    {

        public Action<IOperation, Row> processAction = delegate    { };
        public Action<IOperation> finishedAction = delegate    { };

        [Fact]
        public void    CanPassOnAddedProcessedEvents()
        {
            //Arrange
            var    branching =    new    TestAbstractBranchingOperation();
            const int nOps = 5;
            var    ops    = new IOperation[nOps];
            for    (var i = 0;    i <    nOps; i++)
            {
                ops[i] = MockRepository.GenerateMock<IOperation>();
                ops[i].Expect(x    => x.OnRowProcessed    += processAction);
                branching.Add(ops[i]);
            }

            //Act
            branching.OnRowProcessed +=    processAction;

            //Assert
            foreach(var    op in ops)
                op.VerifyAllExpectations();
            var    handlerInfos = typeof(AbstractOperation).GetField("OnRowProcessed",    BindingFlags.Static    | BindingFlags.Instance    | BindingFlags.NonPublic);
            Assert.Equal(2,    ((Delegate)(handlerInfos.GetValue(branching))).GetInvocationList().Length);
        }

        [Fact]
        public void    CanPassOnAddedFinishedEvents()
        {
            var    branching =    new    TestAbstractBranchingOperation();
            const int nOps = 5;
            var    ops    = new IOperation[nOps];
            for    (var i = 0;    i <    nOps; i++)
            {
                ops[i] = MockRepository.GenerateMock<IOperation>();
                ops[i].Expect(x    => x.OnFinishedProcessing += finishedAction);
                branching.Add(ops[i]);
            }

            branching.OnFinishedProcessing += finishedAction;

            foreach    (var op    in ops)
                op.VerifyAllExpectations();

            var    handlerInfos = typeof(AbstractOperation).GetField("OnFinishedProcessing", BindingFlags.Static |    BindingFlags.Instance |    BindingFlags.NonPublic);
            Assert.Equal(2,    ((Delegate)(handlerInfos.GetValue(branching))).GetInvocationList().Length);
        }

        [Fact]
        public void    CanPassOnRemovedProcessedEvents()
        {
            //Arrange
            var    branching =    new    TestAbstractBranchingOperation();
            const int nOps = 5;
            var    ops    = new IOperation[nOps];
            for    (var i = 0;    i <    nOps; i++)
            {
                ops[i] = MockRepository.GenerateMock<IOperation>();
                ops[i].Expect(x    => x.OnRowProcessed    += processAction);
                ops[i].Expect(x    => x.OnRowProcessed    -= processAction);
                branching.Add(ops[i]);
            }

            //Act
            branching.OnRowProcessed +=    processAction;
            branching.OnRowProcessed -=    processAction;

            //Assert
            foreach    (var op    in ops)
                op.VerifyAllExpectations();

            var    handlerInfos = typeof(AbstractOperation).GetField("OnRowProcessed",    BindingFlags.Static    | BindingFlags.Instance    | BindingFlags.NonPublic);
            Assert.Equal(1,    ((Delegate)(handlerInfos.GetValue(branching))).GetInvocationList().Length);
        }

        [Fact]
        public void    CanPassOnRemovedFinishedEvents()
        {
            var    branching =    new    TestAbstractBranchingOperation();
            const int nOps = 5;
            var    ops    = new IOperation[nOps];
            for    (var i = 0;    i <    nOps; i++)
            {
                ops[i] = MockRepository.GenerateMock<IOperation>();
                ops[i].Expect(x    => x.OnFinishedProcessing += finishedAction);
                ops[i].Expect(x    => x.OnFinishedProcessing -= finishedAction);
                branching.Add(ops[i]);
            }

            branching.OnFinishedProcessing += finishedAction;
            branching.OnFinishedProcessing -= finishedAction;

            foreach    (var op    in ops)
                op.VerifyAllExpectations();

            var    handlerInfos = typeof(AbstractOperation).GetField("OnFinishedProcessing", BindingFlags.Static |    BindingFlags.Instance |    BindingFlags.NonPublic);
            Assert.Equal(1,    ((Delegate)(handlerInfos.GetValue(branching))).GetInvocationList().Length);
        }
    }

    public class TestAbstractBranchingOperation    : AbstractBranchingOperation
    {
        public override    IEnumerable<Row> Execute(IEnumerable<Row> rows)
        {
            throw new NotImplementedException();
        }
    }
}
