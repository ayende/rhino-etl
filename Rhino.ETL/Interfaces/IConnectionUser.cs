namespace Rhino.ETL
{
	public interface IConnectionUser
	{
		Connection ConnectionInstance { get; }
		bool TryAcquireConnection();
		void ReleaseConnection();
	}
}