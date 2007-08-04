using System;
using System.Collections;

namespace Rhino.ETL
{
	public class TransformationBase<T> : ContextfulObjectBase<T>
		where T : TransformationBase<T>
	{
		protected const string DefaultOutputQueue = "Output";
		protected string name;
		protected QueuesManager queuesManager;

		[ThreadStatic]
		public static TransformParameters CurrentTransformParameters;


		public override string Name
		{
			get { return name; }
		}

		public void RemoveRow()
		{
			CurrentTransformParameters.ShouldSkipRow = true;
		}

		public void SendRow(Row row)
		{
			SendRow(CurrentTransformParameters.OutputQueueName, row);
		}

		public void SendRow(string queueName, Row row)
		{
			TransformParameters old = CurrentTransformParameters;
			PrepareCurrentTransformParameters(CurrentTransformParameters.Pipeline, row);
			CurrentTransformParameters.OutputQueueName = queueName;
			ForwardRow();
			CurrentTransformParameters = old;
		}

		public Pipeline Context
		{
			get { return Pipeline.Current; }
		}

		protected static void PrepareCurrentTransformParameters(Pipeline pipeline,Row row)
		{
			CurrentTransformParameters = new TransformParameters();
			CurrentTransformParameters.OutputQueueName = DefaultOutputQueue;
			CurrentTransformParameters.ShouldSkipRow = false;
			CurrentTransformParameters.Row = row;
			CurrentTransformParameters.Pipeline = pipeline;
		}

		protected void ForwardRow()
		{
			if (CurrentTransformParameters.ShouldSkipRow)
				return;
			if (CurrentTransformParameters.Row == null)
				return;
			queuesManager.Forward(CurrentTransformParameters.QueueKey,
								  CurrentTransformParameters.Row);
		}

	}
}