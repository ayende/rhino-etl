using System;
using System.Collections;
using Rhino.ETL.Impl;

namespace Rhino.ETL
{
	public abstract class Transform : ContextfulObjectBase<Transform>, IInputOutput
	{
		private const string DefaultOutputQueue = "Output";

		public class TransformParameters
		{
			public bool ShouldSkipRow;
			public string OutputQueueName;
		}

		[ThreadStatic]
		public static TransformParameters CurrentTransformParameters;

		private string name;
		QueuesManager queuesManager;


		protected Transform(string name)
		{
			this.name = name;
			this.queuesManager = new QueuesManager(name, Logger);
			EtlConfigurationContext.Current.AddTransform(name, this);
		}

		public override string Name
		{
			get { return name; }
		}

		public void RemoveRow()
		{
			CurrentTransformParameters.ShouldSkipRow = true;
		}

		public void Apply(Row row, IDictionary parameters)
		{
			CurrentTransformParameters = new TransformParameters();
			CurrentTransformParameters.OutputQueueName = DefaultOutputQueue;
			CurrentTransformParameters.ShouldSkipRow = false;
			DoApply(row, new QuackingDictionary(parameters));
		}

		public void RegisterForwarding(string inQueue, IOutput output, string outQueue, IDictionary parameters)
		{
			queuesManager.RegisterForwarding(inQueue, output, outQueue, parameters);
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