namespace Rhino.ETL
{
	public interface ICommandWithResult : ICommand
	{
		object Result { get; }
	}
}