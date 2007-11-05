namespace Rhino.ETL
{
	using System;
	using Engine;

	public abstract class TransformationBase<T> : ContextfulObjectBase<T>
		where T : TransformationBase<T>
	{
		protected const string DefaultOutputName = "Output";

		[ThreadStatic] public static TransformParameters CurrentTransformParameters;

		protected string name;

		private string outputName = DefaultOutputName;

		public Pipeline Context
		{
			get { return Pipeline.Current; }
		}


		public string OutputName
		{
			get { return outputName; }
			set { outputName = value; }
		}

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
			PrepareCurrentTransformParameters(row);
			CurrentTransformParameters.OutputQueueName = queueName;
			ForwardRow();
			CurrentTransformParameters = old;
		}

		protected void PrepareCurrentTransformParameters(Row row)
		{
			CurrentTransformParameters = new TransformParameters();
			CurrentTransformParameters.OutputQueueName = OutputName;
			CurrentTransformParameters.ShouldSkipRow = false;
			CurrentTransformParameters.Row = row;
		}

		protected void ForwardRow()
		{
			if (CurrentTransformParameters.ShouldSkipRow)
				return;
			if (CurrentTransformParameters.Row == null)
				return;
			ProcessContextFromCurrentContext.Publish(
				Name + "." + CurrentTransformParameters.OutputQueueName,
				CurrentTransformParameters.Row);
		}
	}
}