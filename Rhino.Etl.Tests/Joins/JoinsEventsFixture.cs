using System;
using System.Collections.Generic;
using System.Reflection;
using Rhino.Etl.Core;
using Rhino.Etl.Core.Operations;
using Rhino.Mocks;
using Xunit;

namespace Rhino.Etl.Tests.Joins
{
	public class JoinEventsFixture
	{

		public Action<IOperation, Row> processAction = delegate	{ };
		public Action<IOperation> finishedAction = delegate	{ };

		[Fact]
		public void	CanPassOnAddedProcessedEvents()
		{
			//Arrange
			var	join = new TestAbstractJoinOperation();
			var	op1	= MockRepository.GenerateMock<IOperation>();
			var	op2	= MockRepository.GenerateMock<IOperation>();
			join.Left(op1).Right(op2);

			op1.Expect(x =>	x.OnRowProcessed +=	processAction);
			op2.Expect(x =>	x.OnRowProcessed +=	processAction);

			//Act
			join.OnRowProcessed	+= processAction;

			//Assert
			op1.VerifyAllExpectations();
			op2.VerifyAllExpectations();

			var	handlerInfos = typeof(AbstractOperation).GetField("OnRowProcessed",	BindingFlags.Static	| BindingFlags.Instance	| BindingFlags.NonPublic);
			Assert.Equal(2,	((Delegate)(handlerInfos.GetValue(join))).GetInvocationList().Length);
		}

		[Fact]
		public void	CanPassOnAddedFinishedEvents()
		{
			var	join = new TestAbstractJoinOperation();
			var	op1	= MockRepository.GenerateMock<IOperation>();
			var	op2	= MockRepository.GenerateMock<IOperation>();
			join.Left(op1).Right(op2);

			op1.Expect(x =>	x.OnFinishedProcessing += finishedAction);
			op2.Expect(x =>	x.OnFinishedProcessing += finishedAction);

			join.OnFinishedProcessing += finishedAction;

			op1.VerifyAllExpectations();
			op2.VerifyAllExpectations();

			var	handlerInfos = typeof(AbstractOperation).GetField("OnFinishedProcessing", BindingFlags.Static |	BindingFlags.Instance |	BindingFlags.NonPublic);
			Assert.Equal(2,	((Delegate)(handlerInfos.GetValue(join))).GetInvocationList().Length);
		}

		[Fact]
		public void	CanPassOnRemovedProcessedEvents()
		{
			//Arrange
			var	join = new TestAbstractJoinOperation();
			var	op1	= MockRepository.GenerateMock<IOperation>();
			var	op2	= MockRepository.GenerateMock<IOperation>();
			join.Left(op1).Right(op2);

			op1.Expect(x =>	x.OnRowProcessed +=	processAction);
			op1.Expect(x =>	x.OnRowProcessed -=	processAction);
			op2.Expect(x =>	x.OnRowProcessed +=	processAction);
			op2.Expect(x =>	x.OnRowProcessed -=	processAction);

			//Act
			join.OnRowProcessed	+= processAction;
			join.OnRowProcessed	-= processAction;

			//Assert
			op1.VerifyAllExpectations();
			op2.VerifyAllExpectations();

			var	handlerInfos = typeof(AbstractOperation).GetField("OnRowProcessed",	BindingFlags.Static	| BindingFlags.Instance	| BindingFlags.NonPublic);
			Assert.Equal(1,	((Delegate)(handlerInfos.GetValue(join))).GetInvocationList().Length);
		}

		[Fact]
		public void	CanPassOnRemovedFinishedEvents()
		{
			var	join = new TestAbstractJoinOperation();
			var	op1	= MockRepository.GenerateMock<IOperation>();
			var	op2	= MockRepository.GenerateMock<IOperation>();
			join.Left(op1).Right(op2);

			op1.Expect(x =>	x.OnFinishedProcessing += finishedAction);
			op1.Expect(x =>	x.OnFinishedProcessing -= finishedAction);
			op2.Expect(x =>	x.OnFinishedProcessing += finishedAction);
			op2.Expect(x =>	x.OnFinishedProcessing -= finishedAction);

			join.OnFinishedProcessing += finishedAction;
			join.OnFinishedProcessing -= finishedAction;

			op1.VerifyAllExpectations();
			op2.VerifyAllExpectations();

			var	handlerInfos = typeof(AbstractOperation).GetField("OnFinishedProcessing", BindingFlags.Static |	BindingFlags.Instance |	BindingFlags.NonPublic);
			Assert.Equal(1,	((Delegate)(handlerInfos.GetValue(join))).GetInvocationList().Length);
		}
	}

	public class TestAbstractJoinOperation : JoinOperation
	{
		public override	IEnumerable<Row> Execute(IEnumerable<Row> rows)
		{
			throw new NotImplementedException();
		}

		protected override Row MergeRows(Row leftRow, Row rightRow)
		{
			throw new NotImplementedException();
		}

		protected override void	SetupJoinConditions()
		{
			throw new NotImplementedException();
		}
	}
}
