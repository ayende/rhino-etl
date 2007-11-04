namespace Rhino.ETL
{
	using Engine;

	public class TransformParameters
	{
		public bool ShouldSkipRow;
		public Row Row;
		public string OutputQueueName;
	}
}