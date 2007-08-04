namespace Rhino.ETL
{
	public interface IConnectionUser
	{
		Connection ConnectionInstance { get; }
		bool TryAcquireConnection(Pipeline pipeline);
		void ReleaseConnection(Pipeline pipeline);
	}
}