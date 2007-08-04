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

		public void Process(QueueKey key, Row row, IDictionary parameters)
		{
			if (key.Name.Equals(LeftQueueName, StringComparison.InvariantCultureIgnoreCase))
			{
				queuesManager.Add(new QueueKey(LeftQueueName, key.Pipeline), row);
			}
			else if (key.Name.Equals(RightQueueName, StringComparison.InvariantCultureIgnoreCase))
			{
				queuesManager.Add(new QueueKey(RightQueueName, key.Pipeline), row);
			}
			else
			{
				throw new ValidationExceptionException("A join may only have a left and right input queues");
			}
		}

		public void Complete(QueueKey key)
		{
			lock (queuesManager)
			{
				if (key.Name.Equals(LeftQueueName, StringComparison.InvariantCultureIgnoreCase))
				{
					queuesManager.SetCompleted(new QueueKey(LeftQueueName, key.Pipeline));
				}
				else if (key.Name.Equals(RightQueueName, StringComparison.InvariantCultureIgnoreCase))
				{
					queuesManager.SetCompleted(new QueueKey(RightQueueName, key.Pipeline));
				}
				else
				{
					throw new ValidationExceptionException("A join may only have a left and right input queues");
				}

				if (queuesManager.IsCompleted(new QueueKey(LeftQueueName, key.Pipeline)) &&
					queuesManager.IsCompleted(new QueueKey(RightQueueName, key.Pipeline)))
				{
					List<Row> left = queuesManager.GetQueue(new QueueKey(LeftQueueName, key.Pipeline));
					List<Row> right = queuesManager.GetQueue(new QueueKey(RightQueueName, key.Pipeline));

					JoinQueues(key.Pipeline, left, right);

					queuesManager.Clear(new QueueKey(LeftQueueName, key.Pipeline));
					queuesManager.Clear(new QueueKey(RightQueueName, key.Pipeline));
					queuesManager.CompleteAll(key.Pipeline);
					queuesManager.ForEachOutputQueue(key.Pipeline, delegate(string outputQueueName)
					{
						Completed(this, new QueueKey(outputQueueName, key.Pipeline));
					});
				}
			}
		}

		private void JoinQueues(Pipeline pipeline, List<Row> left, List<Row> right)
		{
			foreach (Row leftRow in left)
			{
				foreach (Row rightRow in right)
				{
					bool shouldAdd = (bool)Condition.Call(new object[] { leftRow, rightRow });
					if (shouldAdd)
						Apply(pipeline, leftRow, rightRow);
				}
			}
		}

		private void Apply(Pipeline pipeline, Row leftValue, Row rightRow)
		{
			PrepareCurrentTransformParameters(pipeline, new Row());
			DoApply(CurrentTransformParameters.Row, leftValue, rightRow);
			ForwardRow();
		}

		protected abstract void DoApply(Row Row, Row Left, Row Right);
	}
}