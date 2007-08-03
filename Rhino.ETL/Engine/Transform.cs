using System;
using System.Collections;
using Rhino.ETL.Impl;

namespace Rhino.ETL
{
	public abstract class Transform : TransformationBase<Transform>, IInputOutput
	{
			QueuesManager queuesManager;


		protected Transform(string name)
		{
			this.name = name;
			this.queuesManager = new QueuesManager(name, Logger);
			EtlConfigurationContext.Current.AddTransform(name, this);
		}


		public void Apply(Row row, IDictionary parameters)
		{
			CurrentTransformParameters = new TransformParameters();
			CurrentTransformParameters.OutputQueueName = DefaultOutputQueue;
			CurrentTransformParameters.ShouldSkipRow = false;
			DoApply(row, new QuackingDictionary(parameters));
		}

		public void RegisterForwarding(PipeLineStage pipeLineStage)
		{
			queuesManager.RegisterForwarding(pipeLineStage);
		}

		public void Process(string queueName, Row row, IDictionary parameters)
		{
			Apply(row, parameters);
			if (CurrentTransformParameters.ShouldSkipRow)
				return;
			queuesManager.Forward(CurrentTransformParameters.OutputQueueName, row);
		}

		public void Complete(string queueName)
		{
			queuesManager.Complete(queueName);
		}

		protected abstract void DoApply(Row Row, QuackingDictionary Parameters);
	}
}