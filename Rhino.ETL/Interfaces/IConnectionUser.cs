namespace Rhino.ETL
{
	using Engine;

	public interface IConnectionUser
	{
		Connection ConnectionInstance { get; }
		bool TryAcquireConnection(Pipeline pipeline);
		void ReleaseConnection(Pipeline pipeline);
	}
}