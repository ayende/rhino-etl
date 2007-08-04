using System;
using System.Collections;
using System.Collections.Generic;
using Boo.Lang;
using Rhino.ETL.Exceptions;

namespace Rhino.ETL
{
	public abstract class Join : TransformationBase<Join>, IInputOutput
	{
		private const string LeftQueueName = "Left";
		private const string RightQueueName = "Right";
		private ICallable condition;
		public event OutputCompleted Completed = delegate { };

		protected Join(string name)
		{
			this.name = name;
			queuesManager = new QueuesManager(name, Logger);
			EtlConfigurationContext.Current.AddJoin(name, this);
		}

		public QueuesManager QueuesManager
		{
			get { return queuesManager; }
		}

		public ICallable Condition
		{
			get { return condition; }
			set { condition = value; }
		}

		public override string Name
		{
			get { return name; }
		}


		public void RegisterForwarding(PipeLineStage pipeLineStage)
		{
			queuesManager.RegisterForwarding(pipeLineStage);
		}

		public void Process(string queueName, Row row, IDictionary parameters)
		{
			if (queueName.Equals(LeftQueueName, StringComparison.InvariantCultureIgnoreCase))
			{
				queuesManager.Add(LeftQueueName, row);
			}
			else if (queueName.Equals(RightQueueName, StringComparison.InvariantCultureIgnoreCase))
			{
				queuesManager.Add(RightQueueName, row);
			}
			else
			{
				throw new ValidationExceptionException("A join may only have a left and right input queues");
			}
		}

		public void Complete(string queueName)
		{
			lock (queuesManager)
			{
				if (queueName.Equals(LeftQueueName, StringComparison.InvariantCultureIgnoreCase))
				{
					queuesManager.SetCompleted(LeftQueueName);
				}
				else if (queueName.Equals(RightQueueName, StringComparison.InvariantCultureIgnoreCase))
				{
					queuesManager.SetCompleted(RightQueueName);
				}
				else
				{
					throw new ValidationExceptionException("A join may only have a left and right input queues");
				}

				if (queuesManager.IsCompleted(LeftQueueName) && queuesManager.IsCompleted(RightQueueName))
				{
					List<Row> left = queuesManager.GetQueue(LeftQueueName);
					List<Row> right = queuesManager.GetQueue(RightQueueName);

					JoinQueues(left, right);

					queuesManager.Clear(LeftQueueName);
					queuesManager.Clear(RightQueueName);
					queuesManager.CompleteAll();
					queuesManager.ForEachOutputQueue(delegate(string outputQueueName)
					{
						Completed(this, outputQueueName);
					});
				}
			}
		}

		private void JoinQueues(List<Row> left, List<Row> right)
		{
			foreach (Row leftRow in left)
			{
				foreach (Row rightRow in right)
				{
					bool shouldAdd = (bool) Condition.Call(new object[] {leftRow, rightRow});
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