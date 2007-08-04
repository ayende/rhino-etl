using System;
using System.Collections;
using Rhino.ETL.Impl;

namespace Rhino.ETL
{
	public abstract class Transform : TransformationBase<Transform>, IInputOutput
	{
		protected Transform(string name)
		{
			this.name = name;
			this.queuesManager = new QueuesManager(name, Logger);
			EtlConfigurationContext.Current.AddTransform(name, this);
		}


		public void Apply(Row row, IDictionary parameters)
		{
			PrepareCurrentTransformParameters(row);
			DoApply(row, new QuackingDictionary(parameters));
		}

		public void RegisterForwarding(PipeLineStage pipeLineStage)
		{
			queuesManager.RegisterForwarding(pipeLineStage);
		}

		public void Process(string queueName, Row row, IDictionary parameters)
		{
			Apply(row, parameters);
			ForwardRow();
		}

		public void Complete(string queueName)
		{
			OnComplete(queueName);
			queuesManager.Complete(queueName);
		}

		protected virtual void OnComplete(string QueueName)
		{
			
		}

		protected abstract void DoApply(Row Row, QuackingDictionary Parameters);
	}
}