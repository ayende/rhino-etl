namespace Rhino.ETL
{
	public class TransformParameters
	{
		public bool ShouldSkipRow;
		public Row Row;
		public string OutputQueueName;
		public Pipeline Pipeline;

		public QueueKey QueueKey
		{
			get
			{
				return new QueueKey(OutputQueueName, Pipeline);
			}
		}
	}
}