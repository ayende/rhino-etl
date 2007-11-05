namespace Rhino.ETL.Engine
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using Boo.Lang;
	using Interfaces;
	using Retlang;

	public abstract class Join : TransformationBase<Join>, IProcess
	{
		private const string LeftQueueName = "Left";
		private const string RightQueueName = "Right";
		private ICallable condition;
		private bool leftDone = false;
		private bool rightDone = false;
		readonly List<Row> left = new List<Row>();
		readonly List<Row> right = new List<Row>();

		protected Join(string name)
		{
			this.name = name;
			EtlConfigurationContext.Current.AddJoin(name, this);
		}

		[Browsable(false)]
		public ICallable Condition
		{
			get { return condition; }
			set { condition = value; }
		}

		public override string Name
		{
			get { return name; }
		}

		public void Start(IProcessContext context, params string[] inputNames)
		{
			if(inputNames.Length != 2)
				throw new ArgumentException("join must have two input names");
			string leftName = inputNames[0];
			string rightName = inputNames[1];

			EtlConfigurationContext configurationContext = EtlConfigurationContext.Current;
			Pipeline currentPipeline = Pipeline.Current;
			
			Items[ProcessContextKey] = context;
			context.Subscribe<object>(new TopicEquals(leftName + Messages.Done), delegate
			{
				leftDone = true;
				using(configurationContext.EnterContext())
				using(currentPipeline.EnterContext())
					TryToComplete(context);
			});
			context.Subscribe<object>(new TopicEquals(rightName + Messages.Done), delegate
			{
				rightDone = true;
				using (configurationContext.EnterContext())
				using (currentPipeline.EnterContext())
					TryToComplete(context);
			});
			context.Subscribe<Row>(new TopicEquals(leftName), delegate(IMessageHeader header, Row msg)
			{
				left.Add(msg);
			});
			context.Subscribe<Row>(new TopicEquals(rightName), delegate(IMessageHeader header, Row msg)
			{
				right.Add(msg);
			});
		}

		private void TryToComplete(IProcessContext context)
		{
			if (leftDone == false || rightDone == false)
				return;

			JoinQueues(left, right);
			context.Publish(Name +"." + OutputName + Messages.Done, Messages.Done);
			context.Stop();
		}

		private void JoinQueues(IEnumerable<Row> leftRows, IEnumerable<Row> rightRows)
		{
			foreach (Row leftRow in leftRows)
			{
				foreach (Row rightRow in rightRows)
				{
					bool shouldAdd = (bool)Condition.Call(new object[] { leftRow, rightRow });
					if (shouldAdd)
						Apply(leftRow, rightRow);
				}
			}
		}

		private void Apply(Row leftValue, Row rightRow)
		{
			PrepareCurrentTransformParameters(new Row());
			DoApply(CurrentTransformParameters.Row, leftValue, rightRow);
			ForwardRow();
		}

		protected abstract void DoApply(Row Row, Row Left, Row Right);
	}
}