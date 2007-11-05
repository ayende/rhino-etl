namespace Rhino.ETL.Interfaces
{
	using Retlang;

	public interface IProcess
	{
		void Start(IProcessContext context, params string [] inputNames);
		string Name { get; }

		string OutputName { get; set; }
	}
}