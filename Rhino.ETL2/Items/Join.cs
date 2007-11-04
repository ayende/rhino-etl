namespace Rhino.ETL.Engine
{
	using System.Collections.Generic;
	using System.ComponentModel;
	using Boo.Lang;
	using Retlang;

	public abstract class Join : TransformationBase<Join>
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

		public void Start(IProcessContext context, string leftName, string rightName)
		{
			Items[ProcessContextKey] = context;
			context.Subscribe<object>(new TopicEquals(leftName + Messages.Done), delegate
			{
				leftDone = true;
				TryToComplete(context);
			});
			context.Subscribe<object>(new TopicEquals(rightName + Messages.Done), delegate
			{
				rightDone = true;
				TryToComplete(context);
			});
			context.Subscribe<Row>(new TopicEquals(LeftQueueName), delegate(IMessageHeader header, Row msg)
			{
				left.Add(msg);
			});
			context.Subscribe<Row>(new TopicEquals(RightQueueName), delegate(IMessageHeader header, Row msg)
			{
				right.Add(msg);
			});
		}

		private void TryToComplete(IProcessContext context)
		{
			if(leftDone == false || rightDone == false)
				return;

			JoinQueues(left, right);
			context.Publish(OutputName + Messages.Done, Messages.Done);
			context.Stop();
		}

		private void JoinQueues(IEnumerable<Row> left, IEnumerable<Row> right)
		{
			foreach (Row leftRow in left)
			{
				foreach (Row rightRow in right)
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