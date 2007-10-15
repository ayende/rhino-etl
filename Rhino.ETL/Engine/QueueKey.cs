using System;

namespace Rhino.ETL
{
	using Engine;

	public class QueueKey
	{
		private string name;
		private Pipeline pipeline;

		public QueueKey(string name, Pipeline pipeline)
		{
			this.name = name;
			this.pipeline = pipeline;
		}


		public string Name
		{
			get { return name; }
		}

		public Pipeline Pipeline
		{
			get { return pipeline; }
		}


		public override bool Equals(object obj)
		{
			if (this == obj) return true;
			QueueKey queueKey = obj as QueueKey;
			if (queueKey == null) return false;
			return string.Equals(name, queueKey.name,StringComparison.InvariantCultureIgnoreCase) 
				&& Equals(pipeline, queueKey.pipeline);
		}

		public override int GetHashCode()
		{
			return name.ToLowerInvariant().GetHashCode() + 29*pipeline.GetHashCode();
		}
	}
}