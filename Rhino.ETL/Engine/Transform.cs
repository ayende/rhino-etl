using System;
using System.Collections;
using Rhino.ETL.Engine;
using Rhino.ETL.Impl;

namespace Rhino.ETL
{
	public abstract class Transform : TransformationBase<Transform>, IInputOutput
	{
		public event OutputCompleted Completed = delegate { };

		protected Transform(string name)
		{
			this.name = name;
			queuesManager = new QueuesManager(name, Logger);
			EtlConfigurationContext.Current.AddTransform(name, this);
		}


		public void Apply(Pipeline pipeline, Row row, IDictionary parameters)
		{
			PrepareCurrentTransformParameters(pipeline, row);
			DoApply(row, new QuackingDictionary(parameters));
		}

		public void RegisterForwarding(Target target, PipeLineStage pipeLineStage)
		{
			queuesManager.RegisterForwarding(target, pipeLineStage);
		}


		public void Process(QueueKey key, Row row, IDictionary parameters)
		{
			Apply(key.Pipeline, row, parameters);
			ForwardRow();
		}

		public void Complete(QueueKey key)
		{
			OnComplete(key.Name);
			queuesManager.CompleteAll(key.Pipeline);
			queuesManager.ForEachOutputQueue(key.Pipeline, delegate(string outputQueueName)
			{
				Completed(this, new QueueKey(outputQueueName, key.Pipeline));
			});
		}

		protected virtual void OnComplete(string QueueName)
		{

		}

		protected abstract void DoApply(Row Row, QuackingDictionary Parameters);
	}
}