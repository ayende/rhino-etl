namespace Rhino.ETL
{
	using Engine;

	public class PipelineAndRow
	{
		public Row Row;
		public Pipeline Pipeline;

		public PipelineAndRow(Row row, Pipeline pipeline)
		{
			Row = row;
			Pipeline = pipeline;
		}
	}
}